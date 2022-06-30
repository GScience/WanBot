using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WanBot.Api.Util;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// 消息链
    /// </summary>
    [JsonConverter(typeof(MessageChainJsonConverter))]
    public class MessageChain : IEnumerable<BaseChain>
    {
        internal IEnumerable<BaseChain> Chain { get; }

        public MessageChain(IEnumerable<BaseChain> chain)
        {
            Chain = chain;
        }

        public MessageChain()
        {
            Chain = Array.Empty<BaseChain>();
        }

        public IEnumerator<BaseChain> GetEnumerator()
        {
            return Chain.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Chain.GetEnumerator();
        }
    }
}
