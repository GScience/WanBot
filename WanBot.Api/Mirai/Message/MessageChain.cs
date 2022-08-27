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
        /// <summary>
        /// 消息Id
        /// </summary>
        public int? MessageId
        {
            get
            {
                if (Chain.FirstOrDefault() is Source source)
                    return source.Id;
                return null;
            }
        }

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

        public override int GetHashCode()
        {
            var hashCode = 0;
            if (Chain != null)
                foreach (var node in Chain)
                    hashCode ^= node.GetHashCode();
            return hashCode;
        }
    }
}
