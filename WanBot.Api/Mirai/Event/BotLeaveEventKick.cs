using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Bot被踢出一个群
    /// </summary>
    public class BotLeaveEventKick : BaseMiraiEvent
    {
        /// <summary>
        /// Bot被踢出的群的信息
        /// </summary>
        public Group Group { get; set; } = new();

        /// <summary>
        /// Bot被踢后获取操作人的 Member 对象
        /// </summary>
        public Member? Operator { get; set; } = new();
    }
}
