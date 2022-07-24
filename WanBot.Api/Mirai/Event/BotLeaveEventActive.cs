using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Bot主动退出一个群
    /// </summary>
    public class BotLeaveEventActive : BaseMiraiEvent
    {
        /// <summary>
        /// Bot退出的群的信息
        /// </summary>
        public Group Group { get; set; } = new();
    }
}
