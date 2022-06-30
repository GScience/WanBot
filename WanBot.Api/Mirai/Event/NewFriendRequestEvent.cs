using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 添加好友申请
    /// </summary>
    public class NewFriendRequestEvent : BaseEvent
    {
        /// <summary>
        /// 事件标识，响应该事件时的标识
        /// </summary>
        public long EventId { get; set; }

        /// <summary>
        /// 申请人QQ号
        /// </summary>
        public long FromId { get; set; }

        /// <summary>
        /// 申请人如果通过某个群添加好友，该项为该群群号；否则为0
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// 申请人的昵称或群名片
        /// </summary>
        public string Nick { get; set; } = string.Empty;

        /// <summary>
        /// 申请消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
