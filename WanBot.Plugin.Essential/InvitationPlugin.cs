using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;

namespace WanBot.Plugin.Essential
{
    /// <summary>
    /// 邀请插件，允许邀请机器人入群
    /// </summary>
    internal class InvitationPlugin : WanBotPlugin
    {
        public override string PluginName => "Invitation";

        public override string PluginDescription => "允许拉完犊子进入群聊";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        [MiraiEvent<BotInvitedJoinGroupRequestEvent>(Priority.Highest)]
        public async Task OnGroupInvitation(MiraiBot bot, BotInvitedJoinGroupRequestEvent e)
        {
            Logger.Info($"Invited to group [{e.GroupName}({e.GroupId})] by {e.Nick}({e.FromId})");
            await bot.ResponseBotInvitedJoinGroupRequestEventAsync(e, false);
        }
    }
}
