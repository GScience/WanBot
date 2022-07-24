using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 某群入群公告改变
    /// </summary>
    public class GroupEntranceAnnouncementChangeEvent : BaseMiraiEvent
    {
        /// <summary>
        /// 原公告
        /// </summary>
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// 新公告
        /// </summary>
        public string Current { get; set; } = string.Empty;

        /// <summary>
        /// 公告改变的群信息
        /// </summary>
        public Group Group { get; set; } = new();

        /// <summary>
        /// 操作的管理员或群主信息，当null时为Bot操作
        /// </summary>
        public Member? Operator { get; set; }
    }
}
