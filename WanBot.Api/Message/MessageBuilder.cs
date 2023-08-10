using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        /// <summary>
        /// 上传指定path的文件
        /// </summary>
        /// <param name="path">只允许绝对路径</param>
        /// <returns></returns>
        public MessageBuilder ImageByPath(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _chains.Add(new Image { Path = path });
            else
            {
                _chains.Add((ChainGenerator)(
                    (MiraiBot bot, MessageType type)
                    => TrySendImageAsync(type, bot, path).Result));
            }
            return this;
        }

        public MessageBuilder Image(MiraiImage image)
        {
            _chains.Add((ChainGenerator)(
                (MiraiBot bot, MessageType type)
                => TrySendImageAsync(type, image).Result));
            return this;
        }

        public MessageBuilder Image(SKImage image, bool autoDispose = false)
        {
            _chains.Add((ChainGenerator)(
                (MiraiBot bot, MessageType type) 
                => TrySendImageAsync(type, new MiraiImage(bot, image, autoDispose)).Result));
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
        public IEnumerable<BaseChain> Build(MiraiBot bot, MessageType messageType, bool useChainGenerator)
        {
            for (var i = 0; i < _chains.Count; ++i)
            {
                var obj = _chains[i];

                if (obj is Plain plain)
                {
                    while(i + 1 < _chains.Count)
                    {
                        if (_chains[i + 1] is not Plain subPlain)
                            break;
                        plain.Text += subPlain.Text;
                        ++i;
                    }
                    yield return plain;
                }

                else if (obj is BaseChain chain)
                    yield return chain;

                else if (obj is ChainGenerator genFunc && useChainGenerator)
                    yield return genFunc.Invoke(bot, messageType);
            }
        }

        public IEnumerable<BaseChain> Build(MiraiBot bot, MessageType messageType)
        {
            foreach (var obj in Build(bot, messageType, true))
                yield return obj;
        }

        private async Task<BaseChain> TrySendImageAsync(MessageType type, MiraiImage image)
        {
            var imageUrl = await image.SendImageAsync(type);
            var imageChain = new Image { ImageId = imageUrl.id };
            return imageChain;
        }

        private async Task<BaseChain> TrySendImageAsync(MessageType type, MiraiBot bot, string imagePath)
        {
            using var imageFileStream = System.IO.File.OpenRead(imagePath); 
            var typeStr = type.ToString().ToLower();
            BaseChain imageChain;
            try
            {
                var uploadResponse = await bot.UploadImageAsync(typeStr, imageFileStream);
                imageChain = new Image { ImageId = uploadResponse.ImageId };
            }
            catch (Exception ex)
            {
                imageChain = new Plain { Text = $"<Img:{ex.Message}>" };
                bot._logger.Warn($"Failed to send image because {ex}");
            }
            return imageChain;
        }
    }
}
