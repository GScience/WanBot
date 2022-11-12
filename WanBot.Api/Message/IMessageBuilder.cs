using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Message
{
    public interface IMessageBuilder
    {
        public IEnumerable<BaseChain> Build(MiraiBot bot, MessageType messageType);
    }
}
