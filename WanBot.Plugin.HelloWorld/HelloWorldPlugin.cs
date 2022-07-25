using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using System.Linq;
using WanBot.Api.Util;
using System.Text.Json;

namespace WanBot.Plugin.HelloWorld
{
    public class HelloWorldPlugin : WanBotPlugin
    {
        public override string PluginName => "HelloWorld";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private CommandDispatcher _commandDispatcher = new();
        private CommandDispatcher _essentialCommandDispatcher = new();

        private HttpClient _client = new HttpClient();

        public override void PreInit()
        {
            base.PreInit();
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:82.0) Gecko/20100101 Firefox/82.0");

            _commandDispatcher["在"]["哪"]["呢"].Handle = async (b, e) =>
            {
                await e.Sender.ReplyAsync("在~这~呢~");
                return true;
            };

            _essentialCommandDispatcher["get"]["grouplist"].Handle = async (b, e) =>
            {
                var msg = "";
                foreach (var group in (await b.GroupListAsync()).Data!)
                    msg += $"{group.Name}\n";
                await e.Sender.ReplyAsync(msg);
                return true;
            };
            _essentialCommandDispatcher["get"]["friendlist"].Handle = async (b, e) =>
            {
                var msg = "";
                foreach (var friend in (await b.FriendListAsync()).Data!)
                    msg += $"{friend.Nickname}\n";
                await e.Sender.ReplyAsync(msg);
                return true;
            };
        }

        [Command("essential")]
        public async Task OnEssentialCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!await _essentialCommandDispatcher.HandleCommandAsync(bot, commandEvent))
                await commandEvent.Sender.ReplyAsync("语法错误哦");

            
        }

        [Command("一言")]
        public async Task OnDailyCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            await commandEvent.Sender.ReplyAsync(await _client.GetStringAsync("https://v1.hitokoto.cn/?c=a&encode=text"));
        }

        [Command("舔我")]
        public async Task OnDogCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            await commandEvent.Sender.ReplyAsync(await _client.GetStringAsync("http://api.botwl.cn/api/tgrj"));
        }

        [Command("完犊子呢")]
        public async Task OnTestCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!await _commandDispatcher.HandleCommandAsync(bot, commandEvent))
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
            var list = atEvent.Chain.ToArray();
            await atEvent.Sender.ReplyAsync("在这呢");
            atEvent.Blocked = true;
        }
    }
}