using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Network;

namespace WanBot.Api.Mirai.Payload
{
    [HttpApi("groupList")]
    [WsApi("groupList")]
    public class GroupListRequest
    {
        public string SessionKey { get; set; } = string.Empty;
    }

    public class GroupListResponse : Response<List<Group>>
    {
    }
}
