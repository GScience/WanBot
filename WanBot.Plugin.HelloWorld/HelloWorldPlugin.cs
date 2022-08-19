using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using System.Linq;
using WanBot.Api.Util;
using System.Text.Json;
using WanBot.Plugin.Essential.Permission;
using WanBot.Graphic;
using WanBot.Plugin.Essential.Extension;

namespace WanBot.Plugin.HelloWorld
{
    public class HelloWorldPlugin : WanBotPlugin
    {
        public override string PluginName => "HelloWorld";

        public override string PluginAuthor => "WanNeng"; 
        
        public override string PluginDescription => "你好，完犊子，这是第一个完犊子Bot的插件";

        public override Version PluginVersion => Version.Parse("1.0.0");
        public override void Start()
        {
            this.GetBotHelp()
                .Category("活着吗")
                .Command("#完犊子呢", "问问完犊子是否还活着")
                .Command("或者也可以直接问", "完犊子呢")
                .Command("戳一戳完犊子", "戳一戳你")
                .Info("你好，世界，完犊子活了！");

            base.Start();
        }

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

            await nudgeEvent.Sender.NudgeAsync();
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