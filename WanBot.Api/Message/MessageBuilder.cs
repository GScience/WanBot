using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Message
{
    public class MessageBuilder : IMessageBuilder
    {
        private delegate BaseChain ChainGenerator(MiraiBot bot, MessageType type);

        private List<object> _chains = new();

        public MessageBuilder At(long id)
        {
            _chains.Add(new At { Target = id });
            return this;
        }

        public MessageBuilder At(ISender sender)
        {
            _chains.Add(new At { Target = sender.Id });
            return this;
        }

        public MessageBuilder Text(string text)
        {
            if (_chains.Count != 0 && _chains.Last() is Plain plain)
                plain.Text += text;
            else
                _chains.Add(new Plain { Text = text });
            return this;
        }
        public MessageBuilder ImageByUrl(string url)
        {
            _chains.Add(new Image { Url = url });
            return this;
        }

        public MessageBuilder ImageByPath(string path)
        {
            _chains.Add(new Image { Path = path });
            return this;
        }

        public MessageBuilder Image(MiraiImage image)
        {
            _chains.Add((ChainGenerator)(
                (MiraiBot bot, MessageType type) 
                => new Image { ImageId = image.GetImageIdAsync(type).Result }));
            return this;
        }

        public MessageBuilder Image(SKImage image, bool autoDispose = false)
        {
            _chains.Add((ChainGenerator)(
                (MiraiBot bot, MessageType type) 
                => new Image { ImageId = new MiraiImage(bot, image, autoDispose).GetImageIdAsync(type).Result }));
            return this;
        }

        public MessageBuilder Chains(IEnumerable<BaseChain> chains)
        {
            foreach (var chain in chains)
            {
                if (_chains.Count != 0 && 
                    _chains.Last() is Plain lastPlain &&
                    chain is Plain chainPlain)
                    lastPlain.Text += chainPlain.Text;
                else
                    _chains.Add(chain);
            }
            return this;
        }

        /// <summary>
        /// 构建消息
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public IEnumerable<BaseChain> Build(MiraiBot bot, MessageType messageType)
        {
            foreach (var obj in _chains)
            {
                if (obj is BaseChain chain)
                    yield return chain;
                else if (obj is ChainGenerator genFunc)
                    yield return genFunc.Invoke(bot, messageType);
            }
        }
    }
}
