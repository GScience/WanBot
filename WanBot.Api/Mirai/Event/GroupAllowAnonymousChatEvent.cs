using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 匿名聊天
    /// </summary>
    public class GroupAllowAnonymousChatEvent : BaseEvent
    {
        /// <summary>
        /// 原本匿名聊天是否开启
        /// </summary>
        public bool Origin { get; set; }

        /// <summary>
        /// 现在匿名聊天是否开启
        /// </summary>
        public bool Current { get; set; }

        /// <summary>
        /// 匿名聊天状态改变的群信息
        /// </summary>
        public Group Group { get; set; } = new();

        /// <summary>
        /// 操作的管理员或群主信息，当null时为Bot操作
        /// </summary>
        public Member? Operator { get; set; }
    }
}
