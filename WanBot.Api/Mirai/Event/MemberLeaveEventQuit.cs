using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 成员主动离群（该成员不是Bot）
    /// </summary>
    public class MemberLeaveEventQuit : BaseEvent
    {
        /// <summary>
        /// 被踢者的信息
        /// </summary>
        public Member Member { get; set; } = new();
    }
}
