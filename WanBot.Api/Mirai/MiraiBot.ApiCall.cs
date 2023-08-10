using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Hook;
using WanBot.Api.Message;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;
using WanBot.Api.Mirai.Payload;

namespace WanBot.Api.Mirai
{
    public partial class MiraiBot
    {
        private HttpClient _httpClient = new();

        public async Task<AboutResponse> AboutAsync()
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<AboutResponse, AboutRequest>(
                await new AboutRequest()
                {
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<FriendListResponse> FriendListAsync()
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<FriendListResponse, FriendListRequest>(
                await new FriendListRequest()
                {
                    SessionKey = SessionKey
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<GroupListResponse> GroupListAsync()
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<GroupListResponse, GroupListRequest>(
                await new GroupListRequest()
                {
                    SessionKey = SessionKey
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<MemberListResponse> MemberListAsync(long target)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<MemberListResponse, MemberListRequest>(
                await new MemberListRequest()
                {
                    SessionKey = SessionKey,
                    Target = target
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<BotProfileResponse> BotProfileAsync()
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<BotProfileResponse, BotProfileRequest>(
                await new BotProfileRequest()
                {
                    SessionKey = SessionKey
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<FriendProfileResponse> FriendProfileAsync(long target)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<FriendProfileResponse, FriendProfileRequest>(
                await new FriendProfileRequest()
                {
                    SessionKey = SessionKey,
                    Target = target
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<MemberProfileResponse> MemberProfileAsync(long target, long memberId)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<MemberProfileResponse, MemberProfileRequest>(
                await new MemberProfileRequest()
                {
                    SessionKey = SessionKey,
                    Target = target,
                    MemberId = memberId
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<UserProfileResponse> UserProfileAsync(long target)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<UserProfileResponse, UserProfileRequest>(
                await new UserProfileRequest()
                {
                    SessionKey = SessionKey,
                    Target = target
                }.HookAsync(this, HookType.Api));

            return result!;
        }
        public async Task<SendGroupMessageResponse> SendGroupMessageAsync(long target, int? quote, MessageChain messageChain)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendGroupMessageResponse, SendGroupMessageRequest>(
                await new SendGroupMessageRequest()
                {
                    SessionKey = SessionKey,
                    Target = target,
                    Quote = quote,
                    MessageChain = messageChain
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<SendGroupMessageResponse> SendGroupMessageAsync(long target, int? quote, string message)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendGroupMessageResponse, SendGroupMessageRequest>(
                await new SendGroupMessageRequest()
                {
                    SessionKey = SessionKey,
                    Target = target,
                    Quote = quote,
                    MessageChain = new MessageChain(new[] { new Plain { Text = message } })
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<SendGroupMessageResponse> SendGroupMessageAsync(long target, int? quote, IMessageBuilder messageBuilder)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendGroupMessageResponse, SendGroupMessageRequest>(
                await new SendGroupMessageRequest()
                {
                    SessionKey = SessionKey,
                    Target = target,
                    Quote = quote,
                    MessageChain = new MessageChain(messageBuilder.Build(this, MessageType.Group))
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<SendFriendMessageResponse> SendFriendMessageAsync(long target, int? quote, MessageChain messageChain)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendFriendMessageResponse, SendFriendMessageRequest>(
                await new SendFriendMessageRequest()
                {
                    SessionKey = SessionKey,
                    Target = target,
                    Quote = quote,
                    MessageChain = messageChain
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<SendFriendMessageResponse> SendFriendMessageAsync(long target, int? quote, string message)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendFriendMessageResponse, SendFriendMessageRequest>(
                await new SendFriendMessageRequest()
                {
                    SessionKey = SessionKey,
                    Target = target,
                    Quote = quote,
                    MessageChain = new MessageChain(new[] { new Plain { Text = message } })
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<SendFriendMessageResponse> SendFriendMessageAsync(long target, int? quote, IMessageBuilder messageBuilder)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendFriendMessageResponse, SendFriendMessageRequest>(
                await new SendFriendMessageRequest()
                {
                    SessionKey = SessionKey,
                    Target = target,
                    Quote = quote,
                    MessageChain = new MessageChain(messageBuilder.Build(this, MessageType.Group))
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<SendTempMessageResponse> SendTempMessageAsync(long qq, long group, int? quote, MessageChain messageChain)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendTempMessageResponse, SendTempMessageRequest>(
                await new SendTempMessageRequest()
                {
                    SessionKey = SessionKey,
                    QQ = qq,
                    Group = group,
                    Quote = quote,
                    MessageChain = messageChain
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<SendTempMessageResponse> SendTempMessageAsync(long qq, long group, int? quote, string message)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendTempMessageResponse, SendTempMessageRequest>(
                await new SendTempMessageRequest()
                {
                    SessionKey = SessionKey,
                    QQ = qq,
                    Group = group,
                    Quote = quote,
                    MessageChain = new MessageChain(new[] { new Plain { Text = message } })
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<SendTempMessageResponse> SendTempMessageAsync(long qq, long group, int? quote, IMessageBuilder messageBuilder)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendTempMessageResponse, SendTempMessageRequest>(
                await new SendTempMessageRequest()
                {
                    SessionKey = SessionKey,
                    QQ = qq,
                    Group = group,
                    Quote = quote,
                    MessageChain = new MessageChain(messageBuilder.Build(this, MessageType.Group))
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<SendNudgeResponse> SendGroupNudgeAsync(long qq, long group)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendNudgeResponse, SendNudgeRequest>(
                await new SendNudgeRequest()
                {
                    SessionKey = SessionKey,
                    Target = qq,
                    Subject = group,
                    Kind = "Group"
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<SendNudgeResponse> SendFriendNudgeAsync(long qq)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendNudgeResponse, SendNudgeRequest>(
                await new SendNudgeRequest()
                {
                    SessionKey = SessionKey,
                    Target = qq,
                    Subject = qq,
                    Kind = "Friend"
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        public async Task<SendNudgeResponse> SendStrangerNudgeAsync(long qq)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<SendNudgeResponse, SendNudgeRequest>(
                await new SendNudgeRequest()
                {
                    SessionKey = SessionKey,
                    Target = qq,
                    Subject = qq,
                    Kind = "Stranger"
                }.HookAsync(this, HookType.Api));

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
            _logger.Info("Sending image...");
            var result = await adapter.SendAsync<UploadImageResponse, UploadImageRequest>(
                await new UploadImageRequest()
                {
                    SessionKey = SessionKey,
                    Type = type,
                    Img = img
                }.HookAsync(this, HookType.Api));
            _logger.Info("Image sent! Url: {url}, Id: {id}", result?.Url, result?.ImageId);
            var sendResult = await _httpClient.GetAsync(result?.Url);
            if (sendResult.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Image is expired");
            return result!;
        }

        /// <summary>
        /// 批准入群
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseBotInvitedJoinGroupRequestEventResponse> ResponseBotInvitedJoinGroupRequestEventAsync(BotInvitedJoinGroupRequestEvent e, bool isDeny, string message = "")
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<ResponseBotInvitedJoinGroupRequestEventResponse, ResponseBotInvitedJoinGroupRequestEventRequest>(
                await new ResponseBotInvitedJoinGroupRequestEventRequest()
                {
                    SessionKey = SessionKey,
                    EventId = e.EventId,
                    FromId = e.FromId,
                    GroupId = e.GroupId,
                    Operate = isDeny ? 1 : 0,
                    Message = message
                }.HookAsync(this, HookType.Api));

            return result!;
        }

        /// <summary>
        /// 退出群聊
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<QuitResponse> Quit(long groupId)
        {
            var adapter = _adapterDict[typeof(HttpAdapter)];
            var result = await adapter.SendAsync<QuitResponse, QuitRequest>(
                await new QuitRequest()
                {
                    SessionKey = SessionKey,
                    Target = groupId
                }.HookAsync(this, HookType.Api));

            return result!;
        }
    }
}
