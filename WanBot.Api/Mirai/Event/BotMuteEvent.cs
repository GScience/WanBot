using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Bot被禁言
    /// </summary>
    public class BotMuteEvent : BaseMiraiEvent
    {
        /// <summary>
        /// 禁言时长，单位为秒
        /// </summary>
        public int DurationSeconds { get; set; }

        /// <summary>
        /// 操作的管理员或群主信息
        /// </summary>
        public Member Operator { get; set; } = new();
    }
}
