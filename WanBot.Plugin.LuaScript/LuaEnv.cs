using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.LuaScript
{
    /// <summary>
    /// Lua环境
    /// </summary>
    internal class LuaEnv : IDisposable
    {
        private Lua _l = new();

        /// <summary>
        /// 运行Lua代码，并返回结果
        /// </summary>
        /// <param name="script"></param>
        public async Task<string> RunAsync(string script)
        {
            using var cancellToken = new CancellationTokenSource();
            using var runLuaTask = Task.Run(() => Run(script), cancellToken.Token);
            cancellToken.CancelAfter(500);
            var result = await runLuaTask;
            return result.ToString();
        }

        private string Run(string script)
        {
            var compileOption = new LuaCompileOptions()
            {
                ClrEnabled = false
            };

            dynamic env = new LuaTable();
            dynamic sys = new LuaTable();
            sys.time = LuaType.GetType(typeof(DateTime));
            sys.math = LuaType.GetType(typeof(Math));
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
            catch (LuaException e)
            {
                string errLineInfo;
                if (e.Line == 0)
                    errLineInfo = "未知行";
                else
                {
                    var lines = script.Split('\n');
                    errLineInfo = $"{e.Line} => {lines[e.Line - 1]}";
                }
                return $"Lua运行错误，因为{e.Message}\n\t行：{errLineInfo}";
            }

            try
            {
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
