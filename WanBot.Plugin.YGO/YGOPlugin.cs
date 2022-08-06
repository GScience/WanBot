using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Plugin.EssentialPermission;

namespace WanBot.Plugin.YGO
{
    public class YGOPlugin : WanBotPlugin
    {
        public override string PluginName => "YGO";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        [Command("查卡")]
        public async Task OnCommandSearchCard(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasPermission(this, "search"))
                return;
        }
    }
}