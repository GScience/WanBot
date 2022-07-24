using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using System.Linq;
using WanBot.Api.Util;

namespace WanBot.Plugin.HelloWorld
{
    public class HelloWorldPlugin : WanBotPlugin
    {
        public override string PluginName => "HelloWorld";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private CommandDispatcher _commandDispatcher = new();

        public override void PreInit()
        {
            base.PreInit();
            _commandDispatcher["在"]["哪"]["呢"].Handle = async (e) =>
            {
                await e.Sender.ReplyAsync("在~这~呢~");
                return true;
            };
        }

        [Command("完犊子呢")]
        public async Task OnTestCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!await _commandDispatcher.HandleCommandAsync(commandEvent))
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