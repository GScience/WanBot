using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Mirai.Payload
{
    [HttpApi("messageFromId")]
    public class MessageFromIdRequest
    {
        public string SessionKey { get; set; } = string.Empty;

        public int Id { get; set; }
    }

    public class MessageFromIdResponse : BaseEvent
    {
    }
}
