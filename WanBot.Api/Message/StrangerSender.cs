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

        public string InternalName { get; }
        public string DisplayName { get; }

        public StrangerSender(MiraiBot bot, string internalName, string displayName, long qqId, long groupId)
        {
            InternalName = internalName;
            DisplayName = displayName;
            Id = qqId;
            _bot = bot;
            GroupId = groupId;
        }

        public async Task ReplyAsync(MessageChain messageChain)
        {
            await _bot.SendTempMessageAsync(Id, GroupId, null, messageChain);
        }

        public async Task ReplyAsync(string message, int? replyId = null)
        {
            await _bot.SendTempMessageAsync(Id, GroupId, replyId, message);
        }

        public async Task ReplyAsync(IMessageBuilder messageBuilder, int? replyId = null)
        {
            await _bot.SendTempMessageAsync(Id, GroupId, replyId, messageBuilder);
        }

        public async Task NudgeAsync()
        {
            await _bot.SendStrangerNudgeAsync(Id);
        }

        public async Task<Profile> GetProfileAsync()
        {
            return await _bot.MemberProfileAsync(GroupId, Id);
        }

        public override int GetHashCode()
        {
            return (GroupId ^ Id).GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not StrangerSender strangerSender)
                return false;
            return
                strangerSender.Id == Id &&
                strangerSender.GroupId == GroupId;
        }
    }
}
