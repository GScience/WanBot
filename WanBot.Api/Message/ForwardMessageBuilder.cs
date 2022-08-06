using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Message
{
    public class ForwardMessageBuilder : IMessageBuilder
    {
        private List<Func<MessageType, Forward.Node>> _nodeListFactory = new();

        /// <summary>
        /// 创建转发消息
        /// </summary>
        /// <returns></returns>
        public ForwardMessageBuilder Forward(long sender, string senderName, IMessageBuilder msg)
        {
            _nodeListFactory.Add((msgType) => new Forward.Node
            {
                MessageChain = new MessageChain(msg.Build(msgType)),
                SenderId = sender,
                SenderName = senderName,
                Time = (int)DateTime.Now.Ticks,
                MessageId = null
            });
            return this;
        }

        public IEnumerable<BaseChain> Build(MessageType messageType)
        {
            yield return new Forward
            {
                NodeList = BuildNodeList(messageType).ToList()
            };
        }

        private IEnumerable<Forward.Node> BuildNodeList(MessageType messageType)
        {
            foreach (var obj in _nodeListFactory)
                yield return obj.Invoke(messageType);
        }
    }
}
