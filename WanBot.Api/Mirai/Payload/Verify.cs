using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;

namespace WanBot.Api.Mirai.Payload
{
    [HttpApi("verify")]
    public class VerifyRequest
    {
        public int Code { get; set; }
        public string Session { get; set; } = string.Empty;
    }

    public class VerifyResponse
    {
        public int Code { get; set; }
        public string Session { get; set; } = string.Empty;
    }
}
