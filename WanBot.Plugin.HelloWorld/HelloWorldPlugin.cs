using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using System.Linq;
using WanBot.Api.Util;
using System.Text.Json;
using WanBot.Plugin.EssentialPermission;

namespace WanBot.Plugin.HelloWorld
{
    public class HelloWorldPlugin : WanBotPlugin
    {
        public override string PluginName => "HelloWorld";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private HttpClient _client = new HttpClient();

        [Command("舔我")]
        public async Task OnDogCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!commandEvent.Sender.HasCommandPermission(this, "舔我"))
                return;

            await commandEvent.Sender.ReplyAsync(await _client.GetStringAsync("http://api.botwl.cn/api/tgrj"));
        }

        [Command("完犊子呢")]
        public async Task OnTestCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!commandEvent.Sender.HasCommandPermission(this, "完犊子呢"))
                return;

            await commandEvent.Sender.ReplyAsync("在这呢");
        }

        [At]
        public async Task OnAt(MiraiBot bot, AtEventArgs atEvent)
        {
            var list = atEvent.Chain.ToArray();

            await atEvent.Sender.ReplyAsync("干啥？");
        }

        [Nudge]
        public async Task OnNudge(MiraiBot bot, NudgeEventArgs nudgeEvent)
        {
            await nudgeEvent.Sender.Nudge();
        }

        [Regex("完犊子呢")]
        public async Task OnHello1(MiraiBot bot, RegexEventArgs atEvent)
        {
            await atEvent.Sender.ReplyAsync("在这呢");
            atEvent.Blocked = true;
        }
    }
}