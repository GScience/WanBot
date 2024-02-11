using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;
using WanBot.Api.Mirai.Network;

namespace WanBot.Api.Util
{
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(MessageChain))]
    [JsonSerializable(typeof(IEnumerable<BaseChain>))]
    [JsonSerializable(typeof(BaseChain))]
    [JsonSerializable(typeof(BaseMiraiEvent))]
    public partial class MiraiJsonContext : JsonSerializerContext
    {
        
    }
}
