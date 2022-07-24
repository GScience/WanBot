using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Message;
using WanBot.Api.Mirai.Network;

namespace WanBot.Api.Mirai.Payload
{
    /// <summary>
    /// 发送好友消息
    /// </summary>
    [HttpApi("sendFriendMessage", HttpAdapterMethod.PostJson)]
    [WsApi("sendFriendMessage")]
    public class SendFriendMessageRequest
    {
        /// <summary>
        /// 已经激活的Session
        /// </summary>
        public string SessionKey { get; set; } = string.Empty;

        /// <summary>
        /// 发送消息目标好友的QQ号
        /// </summary>
        public long Target { get; set; }

        /// <summary>
        /// 引用一条消息的messageId进行回复
        /// </summary>
        public int? Quote { get; set; }

        public MessageChain? MessageChain { get; set; }
    }

    public class SendFriendMessageResponse : Response
    {
        public int MessageId { get; set; }
    }
}
