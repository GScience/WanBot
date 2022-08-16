using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.RandomStuff
{
    public class RandomStuffPlugin : WanBotPlugin
    {
        public override string PluginName => "RandomStuff";

        public override string PluginDescription => "生成一些与随即相关的有趣内容";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private Random _random = new();

        [Command("随机对象")]
        public async Task OnRandomCp(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "cp"))
                return;

            if (args.Sender is not GroupSender groupSender)
                return;

            var groupMemberList = await bot.MemberListAsync(groupSender.GroupId);

            if (groupMemberList == null || groupMemberList.Data == null)
            {
                Logger.Error("Failed to get member list of group {groupId}", groupSender.GroupId);
                await args.Sender.ReplyAsync("完犊子了，不知道你群里都有谁");
                return;
            }

            var index = _random.Next(0, groupMemberList.Data.Count);
            var msgBuilder = new MessageBuilder();
            msgBuilder.At(groupSender).Text(" 你的对象是：").At(groupMemberList.Data[index].Id);
            await groupSender.ReplyAsync(msgBuilder);

            args.Blocked = true;
        }
    }
}