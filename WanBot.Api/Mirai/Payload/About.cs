﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Network;

namespace WanBot.Api.Mirai.Payload
{
    [HttpApi("about")]
    [WsApi("about")]
    public class AboutRequest : Request
    {
    }

    public class AboutResponse : Response<AboutResponseContent>
    {
    }

    public class AboutResponseContent
    {
        public string Version { get; set; } = string.Empty;
    }
}
