using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 好友昵称改变
    /// </summary>
    public class FriendNickChangedEvent : BaseEvent
    {
        /// <summary>
        /// 好友
        /// </summary>
        public Friend Friend { get; set; } = new();

        /// <summary>
        /// 原昵称
        /// </summary>
        public string From { get; set; } = string.Empty;

        /// <summary>
        /// 新昵称
        /// </summary>
        public string To { get; set; } = string.Empty;
    }
}
