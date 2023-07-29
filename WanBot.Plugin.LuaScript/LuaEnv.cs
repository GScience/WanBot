using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Message;

namespace WanBot.Plugin.LuaScript
{
    /// <summary>
    /// Lua环境
    /// </summary>
    internal class LuaEnv : IDisposable
    {
        private Lua _l = new();

        private HashSet<Type> _whitelistType = new()
        {
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Math),
            typeof(Random),
            typeof(MessageBuilder),
        };

        /// <summary>
        /// 运行Lua代码，并返回结果
        /// </summary>
        /// <param name="script"></param>
        public async Task<object> RunAsync(string script)
        {
            using var cancellToken = new CancellationTokenSource();
            using var runLuaTask = Task.Run(() => Run(script, cancellToken.Token), cancellToken.Token);
            cancellToken.CancelAfter(100);
            var result = await runLuaTask;
            return result;

        }

        private object Run(string script, CancellationToken ct)
        {
            var debugger = new LuaDebugger(ct);
            var compileOption = new LuaCompileOptions()
            {
                ClrEnabled = false,
                DebugEngine = debugger,
                DynamicSandbox = (obj) =>
                {
                    if (obj is LuaType luaType)
                    {
                        if (!_whitelistType.Contains(luaType.Type))
                            return $"<{luaType.FullName} is Blocked>";
                    }
                    return obj;
                }
            };

            dynamic env = new LuaTable();
            dynamic sys = new LuaTable();
            sys.time = LuaType.GetType(typeof(DateTime));
            sys.math = LuaType.GetType(typeof(Math));
            sys.msgBuilder = LuaType.GetType(typeof(MessageBuilder));
            sys.rnd = LuaType.GetType(typeof(Random));
            env.sys = sys;
            env.result = null;

            LuaChunk luaChunk;
            try
            {
                luaChunk = _l.CompileChunk(script, "userCode.lua", compileOption);
            }
            catch (LuaParseException e)
            {
                string errLineInfo;
                if (e.Line == 0)
                    errLineInfo = "未知行";
                else
                {
                    var lines = script.Split('\n');
                    errLineInfo = $"{e.Line} => {lines[e.Line - 1]}";
                }
                return $"Lua编译错误，因为{e.Message}\n\t行：{errLineInfo}";
            }
            try
            {
                luaChunk.Run(env);
            }
            catch (OperationCanceledException)
            {
                return "运行超时";
            }
            catch (Exception e)
            {
                string errLineInfo;
                if (debugger.CurrentLine == 0)
                    errLineInfo = "未知行";
                else
                {
                    var lines = script.Split('\n');
                    errLineInfo = $"{debugger.CurrentLine} => {lines[debugger.CurrentLine - 1]}";
                }
                return $"Lua运行错误，因为{e.Message}\n\t行：{errLineInfo}";
            }

            try
            {
                if (env.result is MessageBuilder messageBuilder)
                    return messageBuilder;
                return LuaValueToString(env.result);
            }
            catch (Exception e)
            {
                return $"无法显示结果，因为{e.Message}";
            }
        }

        private string LuaValueToString(object obj, int depth = 0)
        {
            if (obj is LuaTable table)
            {
                if (depth == 2)
                    return "<Too deep!>";
                var tableStr = "[";
                foreach (var pair in table)
                    tableStr += $"\n{new string('\t', depth + 1)}{LuaValueToString(pair.Key, depth + 1)}={LuaValueToString(pair.Value, depth + 1)}";
                return tableStr + $"\n{new string('\t', depth)}]";
            }
            else if (obj is LuaType type)
            {
                return $"<C# Type{type.FullName}>";
            }
            else if (obj == null)
            {
                return "result is null";
            }
            else return obj.ToString() ?? "";
        }

        public void Dispose()
        {
            _l.Dispose();
        }
    }
}
