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
        public LuaDebugger(CancellationToken ct)
        {
            _ct = ct;
        }

        protected override void OnTracePoint(LuaTraceLineEventArgs e)
        {
            _ct.ThrowIfCancellationRequested();
            base.OnTracePoint(e);
        }
    }
}
