﻿using System;
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
    /// 撤回
    /// </summary>
    [HttpApi("recall", HttpAdapterMethod.PostJson)]
    [WsApi("recall")]
    public class RecallRequest : Request
    {
        /// <summary>
        /// 已经激活的Session
        /// </summary>
        public string SessionKey { get; set; } = string.Empty;

        /// <summary>
        /// 需要撤回的消息的messageId
        /// </summary>
        public int Target { get; set; }
    }

    public class RecallResponse : Response
    {
    }
}
