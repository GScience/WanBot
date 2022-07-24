using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 好友消息
    /// </summary>
    public class FriendMessage : BaseMiraiEvent
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public Friend Sender { get; set; } = new();

        /// <summary>
        /// 消息链
        /// </summary>
        public MessageChain MessageChain { get; set; } = new();
    }
}
