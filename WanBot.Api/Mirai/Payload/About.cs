using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;

namespace WanBot.Api.Mirai.Payload
{
    [HttpApi("about")]
    [WsApi("about")]
    public class AboutRequest
    {
    }

    public class AboutResponse
    {
        public string Version { get; set; } = string.Empty;
    }
}
