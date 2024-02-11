using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Message
{
    public class ForwardMessageBuilder : IMessageBuilder
    {
        private List<Func<MiraiBot, MessageType, Forward.ForwardNode>> _nodeListFactory = new();

        /// <summary>
        /// 创建转发消息
        /// </summary>
        /// <returns></returns>
        public ForwardMessageBuilder Forward(long target, string senderName, IMessageBuilder msg)
        {
            _nodeListFactory.Add((bot, msgType) => new Forward.ForwardNode
            {
                MessageChain = new MessageChain(msg.Build(bot, msgType)),
                SenderId = target,
                SenderName = senderName,
                Time = (int)DateTime.Now.Ticks,
                MessageId = null
            });
            return this;
        }

        public IEnumerable<BaseChain> Build(MiraiBot bot, MessageType messageType)
        {
            yield return new Forward
            {
                NodeList = BuildNodeList(bot, messageType).ToList()
            };
        }

        private IEnumerable<Forward.ForwardNode> BuildNodeList(MiraiBot bot, MessageType messageType)
        {
            foreach (var obj in _nodeListFactory)
                yield return obj.Invoke(bot, messageType);
        }
    }
}
