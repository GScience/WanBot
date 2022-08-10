using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using System.Linq;
using WanBot.Api.Util;
using System.Text.Json;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.HelloWorld
{
    public class HelloWorldPlugin : WanBotPlugin
    {
        public override string PluginName => "HelloWorld";

        public override string PluginAuthor => "WanNeng"; 
        
        public override string PluginDescription => "你好，完犊子，这是第一个完犊子Bot的插件";

        public override Version PluginVersion => Version.Parse("1.0.0");

        [Command("完犊子呢")]
        public async Task OnEchoCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!commandEvent.Sender.HasPermission(this, "echo"))
                return;

            await commandEvent.Sender.ReplyAsync("在这呢");
        }

        [At]
        public async Task OnAt(MiraiBot bot, AtEventArgs atEvent)
        {
            if (!atEvent.Sender.HasPermission(this, "at"))
                return;

            await atEvent.Sender.ReplyAsync("干啥？");
        }

        [Nudge]
        public async Task OnNudge(MiraiBot bot, NudgeEventArgs nudgeEvent)
        {
            if (!nudgeEvent.Sender.HasPermission(this, "nudge"))
                return;

            await nudgeEvent.Sender.Nudge();
        }

        [Regex("完犊子呢")]
        public async Task OnEchoMessage(MiraiBot bot, RegexEventArgs atEvent)
        {
            if (!atEvent.Sender.HasPermission(this, "message"))
                return;

            await atEvent.Sender.ReplyAsync("在这呢");
            atEvent.Blocked = true;
        }
    }
}