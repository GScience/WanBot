using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;

namespace WanBot.Plugin.HelloWorld
{
    public class HelloWorldPlugin : WanBotPlugin
    {
        public override string PluginName => "HelloWorld";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");


        [Command("完犊子呢")]
        public async Task OnTestCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            var messageBuilder = new MessageBuilder().Text("在这呢");
            var remain = commandEvent.GetRemain();
            if (remain == null)
                await commandEvent.Sender.ReplyAsync(messageBuilder);
            else
                await commandEvent.Sender.ReplyAsync(messageBuilder.Text("，你刚才说").Chains(remain).Text("干啥"));
        }
    }
}