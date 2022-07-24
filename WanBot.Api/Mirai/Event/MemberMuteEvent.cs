using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 群成员被禁言事件（该成员不是Bot）
    /// </summary>
    public class MemberMuteEvent : BaseMiraiEvent
    {
        /// <summary>
        /// 禁言时长，单位为秒
        /// </summary>
        public long DurationSeconds { get; set; }

        /// <summary>
        /// 被禁言的群员的信息
        /// </summary>
        public Member Member { get; set; } = new();

        /// <summary>
        /// 操作者的信息，当null时为Bot操作
        /// </summary>
        public Member? Operator { get; set; }
    }
}
