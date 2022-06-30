using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Bot主动离线
    /// </summary>
    public class BotOfflineEventActive : BaseEvent
    {
        /// <summary>
        /// 主动离线的Bot的QQ号
        /// </summary>
        public long QQ { get; set; }
    }
}
