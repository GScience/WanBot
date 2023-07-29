using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.LuaScript
{
    internal class LuaDebugger : LuaTraceLineDebugger
    {
        private CancellationToken _ct;
        public int CurrentLine { get; private set; }

        public LuaDebugger(CancellationToken ct)
        {
            _ct = ct;
        }

        protected override void OnExceptionUnwind(LuaTraceLineExceptionEventArgs e)
        {
            CurrentLine = e.SourceLine;
            base.OnExceptionUnwind(e);
        }
        protected override void OnTracePoint(LuaTraceLineEventArgs e)
        {
            _ct.ThrowIfCancellationRequested();
            CurrentLine = e.SourceLine;
            base.OnTracePoint(e);
        }
    }
}
