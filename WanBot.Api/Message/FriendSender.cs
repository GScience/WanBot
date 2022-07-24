using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Message
{
    public class FriendSender : ISender
    {
        private long _qqId;
        private MiraiBot _bot;

        public string Name { get; }

        public FriendSender(MiraiBot bot, string name, long qqId)
        {
            Name = name;
            _qqId = qqId;
            _bot = bot;
        }

        public async Task ReplyAsync(MessageChain messageChain)
        {
            await _bot.SendFriendMessageAsync(_qqId, null, messageChain);
        }

        public async Task ReplyAsync(string message)
        {
            await _bot.SendFriendMessageAsync(_qqId, null, message);
        }

        public async Task ReplyAsync(MessageBuilder messageBuilder)
        {
            await _bot.SendFriendMessageAsync(_qqId, null, messageBuilder);
        }
    }
}
