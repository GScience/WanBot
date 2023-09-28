using System.Runtime.Caching;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using WanBot.Api.Message;

namespace WanBot.Plugin.JavaScript;

public class JsEnv : IDisposable
{
    public void Dispose() { }

    public async Task<object> RunAsync(string script, TimeSpan timeout, params object[] callArgs)
    {
        using var cancelToken = new CancellationTokenSource();
        using var runLuaTask =
            Task.Run(() => RunAsync(script, cancelToken.Token, callArgs), cancelToken.Token);
        cancelToken.CancelAfter(timeout);
        try
        {
            var result = await runLuaTask;
            return result;
        }
        catch (ScriptInterruptedException)
        {
            return "运行超时或执行中断";
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

    public async Task<object> RunAsync(string script, CancellationToken ct, params object[] callArgs)
    {
        try
        {
            using var engine = new V8ScriptEngine(
                $"{Guid.NewGuid():N}",
                V8ScriptEngineFlags.EnableTaskPromiseConversion // 启用 Task Promise(js) 互转
                | V8ScriptEngineFlags.EnableValueTaskPromiseConversion // 启用 ValueTask Promise(js) 互转
                | V8ScriptEngineFlags.MarshalAllLongAsBigInt // long 作为 BigInt(js)
                | V8ScriptEngineFlags.EnableDateTimeConversion // 启用 DateTime Date(js) 互转
                | V8ScriptEngineFlags.EnableStringifyEnhancements // 启用 c# 类型的 json 序列化
            )
            {
                AllowReflection = false // 禁用反射
            };
            engine.AddHostType(typeof(MessageBuilder));

            engine.ContinuationCallback = () => !ct.IsCancellationRequested;
            ct.Register(() => engine.Interrupt());

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
