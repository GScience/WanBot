using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 成员被踢出群（该成员不是Bot）
    /// </summary>
    public class MemberLeaveEventKick : BaseEvent
    {
        /// <summary>
        /// 被踢者的信息
        /// </summary>
        public Member Member { get; set; } = new();

        /// <summary>
        /// 操作的管理员或群主信息，当null时为Bot操作
        /// </summary>
        public Member? Operator { get; set; }
    }
}
