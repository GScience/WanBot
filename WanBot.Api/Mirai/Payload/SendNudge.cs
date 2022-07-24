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
    /// 发送群消息
    /// </summary>
    [HttpApi("sendNudge", HttpAdapterMethod.PostJson)]
    [WsApi("sendNudge")]
    public class SendNudgeRequest
    {
        /// <summary>
        /// 已经激活的Session
        /// </summary>
        public string SessionKey { get; set; } = string.Empty;

        /// <summary>
        /// 戳一戳的目标, QQ号, 可以为 bot QQ号
        /// </summary>
        public long Target { get; set; }

        /// <summary>
        /// 戳一戳接受主体(上下文), 戳一戳信息会发送至该主体, 为群号/好友QQ号
        /// </summary>
        public long Subject { get; set; }

        /// <summary>
        /// 上下文类型, 可选值 Friend, Group, Stranger
        /// </summary>
        public string Kind { get; set; } = string.Empty;
    }

    public class SendNudgeResponse : Response
    {
    }
}
