using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Network;

namespace WanBot.Api.Mirai.Payload
{
    /// <summary>
    /// 撤回
    /// </summary>
    [HttpApi("resp/botInvitedJoinGroupRequestEvent", HttpAdapterMethod.PostJson)]
    [WsApi("resp_botInvitedJoinGroupRequestEvent")]
    public class ResponseBotInvitedJoinGroupRequestEventRequest : Request
    {
        /// <summary>
        /// 已经激活的Session
        /// </summary>
        public string SessionKey { get; set; } = string.Empty;

        /// <summary>
        /// 事件标识
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
        /// 响应的操作类型
        /// </summary>
        public int Operate { get; set; }

        /// <summary>
        /// 回复的信息
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    public class ResponseBotInvitedJoinGroupRequestEventResponse : Response
    {
    }
}
