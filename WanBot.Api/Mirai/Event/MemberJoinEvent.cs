using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 新人入群的事件
    /// </summary>
    public class MemberJoinEvent : BaseEvent
    {
        /// <summary>
        /// 新人信息
        /// </summary>
        public Member Member { get; set; } = new();

        /// <summary>
        /// 如果被要求入群的话，则为邀请人的 Member 对象
        /// </summary>
        public Member? Invitor { get; set; }
    }
}
