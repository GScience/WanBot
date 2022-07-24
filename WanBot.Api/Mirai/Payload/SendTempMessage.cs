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
    [HttpApi("sendTempMessage", HttpAdapterMethod.PostJson)]
    [WsApi("sendTempMessage")]
    public class SendTempMessageRequest
    {
        /// <summary>
        /// 已经激活的Session
        /// </summary>
        public string SessionKey { get; set; } = string.Empty;

        /// <summary>
        /// 临时会话对象QQ号
        /// </summary>
        public long QQ { get; set; }

        /// <summary>
        /// 临时会话群号
        /// </summary>
        public long Group { get; set; }

        /// <summary>
        /// 引用一条消息的messageId进行回复
        /// </summary>
        public int? Quote { get; set; }

        public MessageChain? MessageChain { get; set; }
    }

    public class SendTempMessageResponse : Response
    {
        public int MessageId { get; set; }
    }
}
