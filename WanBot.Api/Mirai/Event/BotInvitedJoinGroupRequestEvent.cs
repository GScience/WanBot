using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Bot被邀请入群申请
    /// </summary>
    public class BotInvitedJoinGroupRequestEvent : BaseEvent
    {
        /// <summary>
        /// 事件标识，响应该事件时的标识
        /// </summary>
        public long EventId { get; set; }

        /// <summary>
        /// 邀请人（好友）的QQ号
        /// </summary>
        public long FromId { get; set; }

        /// <summary>
        /// 被邀请进入群的群号
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// 被邀请进入群的群名称
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// 邀请人（好友）的昵称
        /// </summary>
        public string Nick { get; set; } = string.Empty;

        /// <summary>
        /// 邀请消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
