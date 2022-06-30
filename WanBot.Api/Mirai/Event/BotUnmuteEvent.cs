using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Bot被取消禁言
    /// </summary>
    public class BotUnmuteEvent : BaseEvent
    {
        /// <summary>
        /// 操作的管理员或群主信息
        /// </summary>
        public Member Operator { get; set; } = new();
    }
}
