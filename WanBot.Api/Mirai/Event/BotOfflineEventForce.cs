using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Bot被挤下线
    /// </summary>
    public class BotOfflineEventForce : BaseEvent
    {
        /// <summary>
        /// 被挤下线的Bot的QQ号
        /// </summary>
        public long QQ { get; set; }
    }
}
