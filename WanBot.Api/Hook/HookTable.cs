using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Event;
using WanBot.Api.Mirai;

namespace WanBot.Api.Hook
{
    /// <summary>
    /// Hook表
    /// </summary>
    public class HookTable
    {
        /// <summary>
        /// 全局Hook Table
        /// </summary>
        public static HookTable Instance { get; } = new();

        /// <summary>
        /// 异常hook
        /// </summary>
        public Func<MiraiBot, Exception, Exception?>? ExceptionHook { get; set; }

        /// <summary>
        /// 事件Hook
        /// </summary>
        public Func<MiraiBot, BlockableEventArgs, BlockableEventArgs>? EventHook { get; set; }

        /// <summary>
        /// Api Hook
        /// </summary>
        public Func<MiraiBot, object, object>? ApiHook { get; set; }
    }
}
