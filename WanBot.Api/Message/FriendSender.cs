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
        public long Id { get; }
        private MiraiBot _bot;

        public string InternalName { get; }
        public string DisplayName { get; }

        public FriendSender(MiraiBot bot, string internalName, string displayName, long qqId)
        {
            InternalName = internalName;
            DisplayName = displayName;
            Id = qqId;
            _bot = bot;
        }

        public async Task ReplyAsync(MessageChain messageChain)
        {
            await _bot.SendFriendMessageAsync(Id, null, messageChain);
        }

        public async Task ReplyAsync(string message, int? replyId = null)
        {
            await _bot.SendFriendMessageAsync(Id, replyId, message);
        }

        public async Task ReplyAsync(IMessageBuilder messageBuilder, int? replyId = null)
        {
            await _bot.SendFriendMessageAsync(Id, replyId, messageBuilder);
        }

        public async Task NudgeAsync()
        {
            await _bot.SendFriendNudgeAsync(Id);
        }

        public async Task<Profile> GetProfileAsync()
        {
            return await _bot.FriendProfileAsync(Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not FriendSender friendSender)
                return false;
            return
                friendSender.Id == Id;
        }
    }
}
