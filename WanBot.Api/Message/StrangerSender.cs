using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Message
{
    public class StrangerSender : ISender
    {
        private long _qqId;
        private long _groupId;
        private MiraiBot _bot;

        public string Name { get; }

        public StrangerSender(MiraiBot bot, string name, long qqId, long groupId)
        {
            Name = name;
            _qqId = qqId;
            _bot = bot;
            _groupId = groupId;
        }

        public async Task ReplyAsync(MessageChain messageChain)
        {
            await _bot.SendTempMessageAsync(_qqId, _groupId, null, messageChain);
        }

        public async Task ReplyAsync(string message)
        {
            await _bot.SendTempMessageAsync(_qqId, _groupId, null, message);
        }

        public async Task ReplyAsync(MessageBuilder messageBuilder)
        {
            await _bot.SendTempMessageAsync(_qqId, _groupId, null, messageBuilder);
        }
    }
}
