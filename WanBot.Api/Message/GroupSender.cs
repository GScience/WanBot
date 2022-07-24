using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Message
{
    /// <summary>
    /// 群成员发送的事件
    /// </summary>
    public class GroupSender : ISender
    {
        private long _groupId;
        private long _qqId;
        private MiraiBot _bot;

        public string Name { get; }

        public GroupSender(MiraiBot bot, string name, long groupId, long qqId)
        {
            Name = name;
            _groupId = groupId;
            _bot = bot;
            _qqId = qqId;
        }

        public async Task ReplyAsync(MessageChain messageChain)
        {
            await _bot.SendGroupMessageAsync(_groupId, null, messageChain);
        }

        public async Task ReplyAsync(string message)
        {
            await _bot.SendGroupMessageAsync(_groupId, null, message);
        }

        public async Task ReplyAsync(MessageBuilder messageBuilder)
        {
            await _bot.SendGroupMessageAsync(_groupId, null, messageBuilder);
        }

        public async Task Nudge()
        {
            await _bot.SendGroupNudgeAsync(_qqId, _groupId);
        }
    }
}
