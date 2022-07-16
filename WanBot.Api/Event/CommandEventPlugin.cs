using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Event
{
    /// <summary>
    /// 命令支持插件
    /// </summary>
    public class CommandEventPlugin : BaseWanBotPlugin
    {
        [MiraiEvent<GroupMessage>(Priority.Highest)]
        public async Task OnGroupMessage(MiraiBot sender, GroupMessage e)
        {
            if (e.MessageChain.Compare("犊子开发机呢"))
                await sender.SendGroupMessageAsync(e.Sender.Group.Id, null, new MessageChain(new[] { new Plain { Text = "在这呢！" } }));

            var chain = e.MessageChain;

            if (!chain.StartsWith("#"))
                return;

            var messageDivider = new MessageChainDivider(e.MessageChain);
            if (messageDivider.ReadNext() is string str && str == "#复读测试")
                await sender.SendGroupMessageAsync(e.Sender.Group.Id, null, new MessageChain(messageDivider.ReadAll()));
        }
    }
}
