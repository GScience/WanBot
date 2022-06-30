using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 群消息撤回
    /// </summary>
    public class GroupRecallEvent : BaseEvent
    {
        /// <summary>
        /// 原消息发送者的QQ号
        /// </summary>
        public long AuthorId { get; set; }

        /// <summary>
        /// 原消息messageId
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// 原消息发送时间
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// 消息撤回所在的群
        /// </summary>
        public Group Group { get; set; } = new();

        /// <summary>
        /// 撤回消息的操作人，当null时为bot操作
        /// </summary>
        public Member? Operator { get; set; }
    }
}
