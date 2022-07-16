using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Message;
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

        public async Task<SendGroupMessageResponse> SendGroupMessageAsync(long target, int? quote, string message)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendGroupMessageResponse, SendGroupMessageRequest>(
                new SendGroupMessageRequest()
                {
                    SessionKey = SessionKey,
                    Target = target,
                    Quote = quote,
                    MessageChain = new MessageChain(new[] { new Plain { Text = message } })
                });

            return result!;
        }

        public async Task<SendGroupMessageResponse> SendGroupMessageAsync(long target, int? quote, MessageBuilder messageBuilder)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendGroupMessageResponse, SendGroupMessageRequest>(
                new SendGroupMessageRequest()
                {
                    SessionKey = SessionKey,
                    Target = target,
                    Quote = quote,
                    MessageChain = new MessageChain(messageBuilder.Build(MessageType.Group))
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

        public async Task<MemberListResponse> MemberListAsync(long target)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<MemberListResponse, MemberListRequest>(
                new MemberListRequest()
                {
                    SessionKey = SessionKey,
                    Target = target
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

        public async Task<FriendProfileResponse> FriendProfileAsync(long target)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<FriendProfileResponse, FriendProfileRequest>(
                new FriendProfileRequest()
                {
                    SessionKey = SessionKey,
                    Target = target
                });

            return result!;
        }

        public async Task<MemberProfileResponse> MemberProfileAsync(long target, long memberId)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<MemberProfileResponse, MemberProfileRequest>(
                new MemberProfileRequest()
                {
                    SessionKey = SessionKey,
                    Target = target,
                    MemberId = memberId
                });

            return result!;
        }

        public async Task<UserProfileResponse> UserProfileAsync(long target)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<UserProfileResponse, UserProfileRequest>(
                new UserProfileRequest()
                {
                    SessionKey = SessionKey,
                    Target = target
                });

            return result!;
        }

        /// <summary>
        /// 上传图像
        /// </summary>
        /// <param name="type">friend或group或temp</param>
        /// <param name="img"></param>
        /// <returns></returns>
        public async Task<UploadImageResponse> UploadImageAsync(string type, Stream img)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<UploadImageResponse, UploadImageRequest>(
                new UploadImageRequest()
                {
                    SessionKey = SessionKey,
                    Type = type,
                    Img = img
                });

            return result!;
        }
    }
}
