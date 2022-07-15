using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Network;

namespace WanBot.Api.Mirai.Payload
{
    [HttpApi("botProfile")]
    [WsApi("botProfile")]
    public class BotProfileRequest
    {
        public string SessionKey { get; set; } = string.Empty;
    }

    public class BotProfileResponse : Profile, IResponse
    {
        
    }
}
