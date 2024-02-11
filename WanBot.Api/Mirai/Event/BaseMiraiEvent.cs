using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WanBot.Api.Event;
using WanBot.Api.Util;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 事件基类
    /// </summary>
    [JsonDerivedType(typeof(BotGroupPermissionChangeEvent), nameof(BotGroupPermissionChangeEvent))]
    [JsonDerivedType(typeof(BotInvitedJoinGroupRequestEvent), nameof(BotInvitedJoinGroupRequestEvent))]
    [JsonDerivedType(typeof(BotJoinGroupEvent), nameof(BotJoinGroupEvent))]
    [JsonDerivedType(typeof(BotLeaveEventActive), nameof(BotLeaveEventActive))]
    [JsonDerivedType(typeof(BotLeaveEventKick), nameof(BotLeaveEventKick))]
    [JsonDerivedType(typeof(BotMuteEvent), nameof(BotMuteEvent))]
    [JsonDerivedType(typeof(BotOfflineEventActive), nameof(BotOfflineEventActive))]
    [JsonDerivedType(typeof(BotOfflineEventDropped), nameof(BotOfflineEventDropped))]
    [JsonDerivedType(typeof(BotOfflineEventForce), nameof(BotOfflineEventForce))]
    [JsonDerivedType(typeof(BotOnlineEvent), nameof(BotOnlineEvent))]
    [JsonDerivedType(typeof(BotReloginEvent), nameof(BotReloginEvent))]
    [JsonDerivedType(typeof(BotUnmuteEvent), nameof(BotUnmuteEvent))]
    [JsonDerivedType(typeof(CommandExecutedEvent), nameof(CommandExecutedEvent))]
    [JsonDerivedType(typeof(FriendInputStatusChangedEvent), nameof(FriendInputStatusChangedEvent))]
    [JsonDerivedType(typeof(FriendMessage), nameof(FriendMessage))]
    [JsonDerivedType(typeof(FriendNickChangedEvent), nameof(FriendNickChangedEvent))]
    [JsonDerivedType(typeof(FriendRecallEvent), nameof(FriendRecallEvent))]
    [JsonDerivedType(typeof(FriendSyncMessage), nameof(FriendSyncMessage))]
    [JsonDerivedType(typeof(GroupAllowAnonymousChatEvent), nameof(GroupAllowAnonymousChatEvent))]
    [JsonDerivedType(typeof(GroupAllowConfessTalkEvent), nameof(GroupAllowConfessTalkEvent))]
    [JsonDerivedType(typeof(GroupAllowMemberInviteEvent), nameof(GroupAllowMemberInviteEvent))]
    [JsonDerivedType(typeof(GroupEntranceAnnouncementChangeEvent), nameof(GroupEntranceAnnouncementChangeEvent))]
    [JsonDerivedType(typeof(GroupMessage), nameof(GroupMessage))]
    [JsonDerivedType(typeof(GroupMuteAllEvent), nameof(GroupMuteAllEvent))]
    [JsonDerivedType(typeof(GroupNameChangeEvent), nameof(GroupNameChangeEvent))]
    [JsonDerivedType(typeof(GroupRecallEvent), nameof(GroupRecallEvent))]
    [JsonDerivedType(typeof(GroupSyncMessage), nameof(GroupSyncMessage))]
    [JsonDerivedType(typeof(MemberCardChangeEvent), nameof(MemberCardChangeEvent))]
    [JsonDerivedType(typeof(MemberHonorChangeEvent), nameof(MemberHonorChangeEvent))]
    [JsonDerivedType(typeof(MemberJoinEvent), nameof(MemberJoinEvent))]
    [JsonDerivedType(typeof(MemberJoinRequestEvent), nameof(MemberJoinRequestEvent))]
    [JsonDerivedType(typeof(MemberLeaveEventKick), nameof(MemberLeaveEventKick))]
    [JsonDerivedType(typeof(MemberLeaveEventQuit), nameof(MemberLeaveEventQuit))]
    [JsonDerivedType(typeof(MemberMuteEvent), nameof(MemberMuteEvent))]
    [JsonDerivedType(typeof(MemberPermissionChangeEvent), nameof(MemberPermissionChangeEvent))]
    [JsonDerivedType(typeof(MemberSpecialTitleChangeEvent), nameof(MemberSpecialTitleChangeEvent))]
    [JsonDerivedType(typeof(MemberUnmuteEvent), nameof(MemberUnmuteEvent))]
    [JsonDerivedType(typeof(NewFriendRequestEvent), nameof(NewFriendRequestEvent))]
    [JsonDerivedType(typeof(NudgeEvent), nameof(NudgeEvent))]
    [JsonDerivedType(typeof(OtherClientMessage), nameof(OtherClientMessage))]
    [JsonDerivedType(typeof(OtherClientOfflineEvent), nameof(OtherClientOfflineEvent))]
    [JsonDerivedType(typeof(OtherClientOnlineEvent), nameof(OtherClientOnlineEvent))]
    [JsonDerivedType(typeof(StrangerMessage), nameof(StrangerMessage))]
    [JsonDerivedType(typeof(StrangerSyncMessage), nameof(StrangerSyncMessage))]
    [JsonDerivedType(typeof(TempMessage), nameof(TempMessage))]
    [JsonDerivedType(typeof(TempSyncMessage), nameof(TempSyncMessage))]
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    public class BaseMiraiEvent : BlockableEventArgs
    {
    }
}
