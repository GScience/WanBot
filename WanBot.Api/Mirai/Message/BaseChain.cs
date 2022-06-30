using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WanBot.Api.Util;

namespace WanBot.Api.Mirai.Message
{
    [JsonConverter(typeof(PolymorphicJsonConverter<BaseChain>))]
    public abstract class BaseChain : ISerializablePolymorphic
    {
        [JsonPropertyOrder(int.MinValue)]
        public string Type => GetType().Name;
    }
}
