using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Message;
using WanBot.Api.Mirai.Payload;

namespace WanBot.Api.Mirai
{
    public partial class MiraiBot
    {
        public async Task<AboutResponse> AboutAsync()
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<AboutResponse, AboutRequest>(
                new AboutRequest()
                {
                });

            return result!;
        }

        public async Task<SendGroupMessageResponse> SendGroupMessageAsync(long target, int? quote, MessageChain messageChain)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendGroupMessageResponse, SendGroupMessageRequest>(
                new SendGroupMessageRequest()
                {
                    SessionKey = SessionKey,
                    Target = target,
                    Quote = quote,
                    MessageChain = messageChain
                });

            return result!;
        }

        public async Task<FriendListResponse> FriendListAsync()
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<FriendListResponse, FriendListRequest>(
                new FriendListRequest()
                {
                    SessionKey = SessionKey
                });

            return result!;
        }

        public async Task<GroupListResponse> GroupListAsync()
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<GroupListResponse, GroupListRequest>(
                new GroupListRequest()
                {
                    SessionKey = SessionKey
                });

            return result!;
        }

        public async Task<BotProfileResponse> BotProfileAsync()
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<BotProfileResponse, BotProfileRequest>(
                new BotProfileRequest()
                {
                    SessionKey= SessionKey
                });

            return result!;
        }
    }
}
