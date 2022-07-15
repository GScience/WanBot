using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;

namespace WanBot.Api.Mirai.Payload
{
    [HttpApi("groupList")]
    public class GroupListRequest
    {
        public string SessionKey { get; set; } = string.Empty;
    }

    public class GroupListResponse : List<Group>
    {
    }
}
