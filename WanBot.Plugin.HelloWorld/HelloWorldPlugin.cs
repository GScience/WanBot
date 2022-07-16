using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;

namespace WanBot.Plugin.HelloWorld
{
    public class HelloWorldPlugin : BaseWanBotPlugin
    {
        [MiraiEvent<GroupMessage>]
        public void OnGroupMessage(MiraiBot bot, MiraiEventArgs<GroupMessage> groupMessage)
        {

        }
    }
}