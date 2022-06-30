using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Bot被服务器断开或因网络问题而掉线
    /// </summary>
    public class BotOfflineEventDropped : BaseEvent
    {
        /// <summary>
        /// 被服务器断开或因网络问题而掉线的Bot的QQ号
        /// </summary>
        public long QQ { get; set; }
    }
}
