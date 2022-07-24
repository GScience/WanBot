using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 好友消息撤回
    /// </summary>
    public class FriendRecallEvent : BaseMiraiEvent
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
        /// 好友QQ号或BotQQ号
        /// </summary>
        public long Operator { get; set; }
    }
}
