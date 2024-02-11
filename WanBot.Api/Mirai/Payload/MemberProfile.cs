using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Network;

namespace WanBot.Api.Mirai.Payload
{
    [HttpApi("memberProfile")]
    [WsApi("memberProfile")]
    public class MemberProfileRequest : Request
    {
        public string SessionKey { get; set; } = string.Empty;
        public long Target { get; set; }
        public long MemberId { get; set; }
    }

    public class MemberProfileResponse : Profile, IResponse
    {
        
    }
}
