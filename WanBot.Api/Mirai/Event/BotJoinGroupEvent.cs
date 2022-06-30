using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Bot加入了一个新群
    /// </summary>
    public class BotJoinGroupEvent : BaseEvent
    {
        /// <summary>
        /// Bot新加入群的信息
        /// </summary>
        public Group Group { get; set; } = new();

        /// <summary>
        /// 如果被邀请入群的话，则为邀请人的 Member 对象
        /// </summary>
        public Member? Invitor { get; set; }
    }
}
