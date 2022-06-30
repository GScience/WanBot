using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Bot主动重新登录
    /// </summary>
    public class BotReloginEvent : BaseEvent
    {
        /// <summary>
        /// 主动重新登录的Bot的QQ号
        /// </summary>
        public long QQ { get; set; }
    }
}
