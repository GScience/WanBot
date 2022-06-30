using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Util
{
    /// <summary>
    /// 消息链序列化
    /// </summary>
    internal class MessageChainJsonConverter : JsonConverter<MessageChain>
    {
        public override MessageChain? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var messageChainArray = JsonSerializer.Deserialize<BaseChain[]>(ref reader, options);
            if (messageChainArray == null)
                return null;
            return new MessageChain(messageChainArray);
        }

        public override void Write(Utf8JsonWriter writer, MessageChain value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Chain, options);
        }
    }
}
