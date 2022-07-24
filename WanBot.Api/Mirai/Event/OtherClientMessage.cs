using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 其他设备消息
    /// </summary>
    public class OtherClientMessage : BaseMiraiEvent
    {
        /// <summary>
        /// 其他客户端
        /// </summary>
        public Client Sender { get; set; } = new();

        /// <summary>
        /// 消息链
        /// </summary>
        public MessageChain MessageChain { get; set; } = new();
    }
}
