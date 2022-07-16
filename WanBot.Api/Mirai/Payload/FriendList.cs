using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Network;

namespace WanBot.Api.Mirai.Payload
{
    [HttpApi("friendList")]
    [WsApi("friendList")]
    public class FriendListRequest
    {
        public string SessionKey { get; set; } = string.Empty;
    }

    public class FriendListResponse : Response<List<Friend>>
    {
    }
}
