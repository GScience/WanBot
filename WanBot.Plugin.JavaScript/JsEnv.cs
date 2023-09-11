using System.Runtime.Caching;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using WanBot.Api.Message;

namespace WanBot.Plugin.JavaScript;

public class JsEnv : IDisposable
{
    private readonly MemoryCache engineCache = new("JavaScriptEngineCache");

    private readonly TimeSpan expiration = new(0, 1, 0, 0);

    private sealed record CacheStub(string session) : IDisposable
    {
        private readonly object locker = new();
        private V8ScriptEngine? engine;

        public V8ScriptEngine Get()
        {
            lock (locker)
            {
                if (engine == null)
                {
                    engine = new V8ScriptEngine(
                        session,
                        V8ScriptEngineFlags.EnableTaskPromiseConversion
                        | V8ScriptEngineFlags.EnableValueTaskPromiseConversion
                        | V8ScriptEngineFlags.MarshalAllLongAsBigInt
                        | V8ScriptEngineFlags.EnableDateTimeConversion
                        | V8ScriptEngineFlags.DisableGlobalMembers
                        | V8ScriptEngineFlags.EnableStringifyEnhancements
                    )
                    {
                        AllowReflection = false
                    };
                    engine.AddHostType(typeof(MessageBuilder));
                }
                return engine;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            var engine = Interlocked.Exchange(ref this.engine, null);
            engine?.Dispose();
        }

        ~CacheStub() => Dispose();
    }

    public void Dispose()
    {
        engineCache.Dispose();
    }

    private CacheStub GetEngine(string session)
    {
        var obj = engineCache.Get(session);
        if (obj != null!) return (CacheStub)obj;
        var stub = new CacheStub(session);
        return (CacheStub?)engineCache.AddOrGetExisting(session, stub, DateTimeOffset.Now + expiration) ?? stub;
    }

    public void Reset(string session)
    {
        engineCache.Remove(session);
    }

    public async Task<object> RunAsync(string session, string script, TimeSpan timeout, params object[] callArgs)
    {
        using var cancelToken = new CancellationTokenSource();
        using var runLuaTask =
            Task.Run(() => RunAsync(session, script, cancelToken.Token, callArgs), cancelToken.Token);
        cancelToken.CancelAfter(timeout);
        try
        {
            var result = await runLuaTask;
            return result;
        }
        catch (OperationCanceledException)
        {
            return "运行超时";
        }
        catch (Exception e)
        {
            return $"Js 运行错误：{e.Message}";
        }
    }

    public async Task<object> RunAsync(string session, string script, CancellationToken ct, params object[] callArgs)
    {
        try
        {
            var engine = GetEngine(session).Get();

            var result = engine.Evaluate(script);

            ct.ThrowIfCancellationRequested();

            if (result == Undefined.Value) return "undefined";

            if (result is Task t) await t;

            ct.ThrowIfCancellationRequested();

            try
            {
                dynamic a = result;
                result = a.Result;
            }
            catch
            {
                // ignored
            }

            return $"{result}";
        }
        catch (Exception e)
        {
            return $"Js 运行错误：{e.Message}";
        }
    }
}
