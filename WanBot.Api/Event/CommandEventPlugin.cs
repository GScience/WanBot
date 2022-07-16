using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;

namespace WanBot.Api.Event
{
    /// <summary>
    /// 命令支持插件
    /// </summary>
    public class CommandEventPlugin : BaseWanBotPlugin
    {
        [MiraiEvent<GroupMessage>(Priority.Highest)]
        public async Task OnGroupMessage(MiraiBot sender, MiraiEventArgs<GroupMessage> e)
        {
            await Task.Delay(0);
        }
    }
}
