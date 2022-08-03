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
        public long GroupId { get; }
        public long Id { get; }
        private MiraiBot _bot;

        public string Name { get; }

        public StrangerSender(MiraiBot bot, string name, long qqId, long groupId)
        {
            Name = name;
            Id = qqId;
            _bot = bot;
            GroupId = groupId;
        }

        public async Task ReplyAsync(MessageChain messageChain)
        {
            await _bot.SendTempMessageAsync(Id, GroupId, null, messageChain);
        }

        public async Task ReplyAsync(string message)
        {
            await _bot.SendTempMessageAsync(Id, GroupId, null, message);
        }

        public async Task ReplyAsync(MessageBuilder messageBuilder)
        {
            await _bot.SendTempMessageAsync(Id, GroupId, null, messageBuilder);
        }

        public async Task Nudge()
        {
            await _bot.SendStrangerNudgeAsync(Id);
        }
    }
}
