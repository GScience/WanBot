using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 某个群名改变
    /// </summary>
    public class GroupNameChangeEvent : BaseEvent
    {
        /// <summary>
        /// 原群名
        /// </summary>
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// 新群名
        /// </summary>
        public string Current { get; set; } = string.Empty;

        /// <summary>
        /// 群名改名的群信息
        /// </summary>
        public Group Group { get; set; } = new();

        /// <summary>
        /// 操作的管理员或群主信息，当null时为Bot操作
        /// </summary>
        public Member? Operator { get; set; }
    }
}
