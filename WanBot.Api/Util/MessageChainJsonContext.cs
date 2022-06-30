using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Util
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(MessageChain))]
    [JsonSerializable(typeof(BaseChain))]
    [JsonSerializable(typeof(BaseEvent))]
    public partial class MiraiJsonContext : JsonSerializerContext
    {
        
    }
}
