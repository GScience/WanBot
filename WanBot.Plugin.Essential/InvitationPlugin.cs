using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using WanBot.Plugin.Essential.Extension;

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

        public override void Start()
        {
            this.GetBotHelp()
                .Category("不喜欢完犊子了？")
                .Command("#滚蛋", "让完犊子退群，需要管理员执行此命令")
                .Info("我滚，我滚，我滚");

            base.Start();
        }

        [Command("滚蛋")] 
        public async Task OnLeaveCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (args.Sender is not GroupSender groupSender)
                return;
            args.Blocked = true;
            if (groupSender.GroupPermission == "MEMBER" &&
                !Permission.Permission.IsAdmin(args.Sender.Id))
            {
                await groupSender.ReplyAsync("请让本群管理员或群主执行此命令");
                return;
            }
            await groupSender.ReplyAsync("好的，我滚");
            Logger.Info($"Leaving group {groupSender.GroupId}");

            try
            {
                await bot.Quit(groupSender.GroupId);
            }
            catch (Exception ex)
            {
                await groupSender.ReplyAsync($"坏了，滚不了，因为{ex.Message}");
            }
        }
    }
}
