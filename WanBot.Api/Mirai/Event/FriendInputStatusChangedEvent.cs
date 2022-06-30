using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 好友输入状态改变
    /// </summary>
    public class FriendInputStatusChangedEvent : BaseEvent
    {
        /// <summary>
        /// 好友
        /// </summary>
        public Friend Friend { get; set; } = new();

        /// <summary>
        /// 当前输出状态是否正在输入
        /// </summary>
        public bool Inputting { get; set; }
    }
}
