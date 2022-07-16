using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Message
{
    public class MessageBuilder
    {
        private delegate BaseChain ChainGenerator(MessageType type);

        private List<object> _chains = new();

        public MessageBuilder Text(string text)
        {
            _chains.Add(new Plain { Text = text });
            return this;
        }

        public MessageBuilder Image(string path)
        {
            _chains.Add(new Image { Path = path });
            return this;
        }

        public MessageBuilder Image(MiraiImage image)
        {
            _chains.Add((ChainGenerator)((MessageType type) => new Image { ImageId = image.GetImageIdAsync(type).Result }));
            return this;
        }

        /// <summary>
        /// 构建消息
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        internal IEnumerable<BaseChain> Build(MessageType messageType)
        {
            foreach (var obj in _chains)
            {
                if (obj is BaseChain chain)
                    yield return chain;
                else if (obj is ChainGenerator genFunc)
                    yield return genFunc.Invoke(messageType);
            }
        }
    }
}
