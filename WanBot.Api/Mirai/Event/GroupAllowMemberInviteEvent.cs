using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 允许群员邀请好友加群
    /// </summary>
    public class GroupAllowMemberInviteEvent : BaseMiraiEvent
    {
        /// <summary>
        /// 原本是否允许群员邀请好友加群
        /// </summary>
        public bool Origin { get; set; }

        /// <summary>
        /// 现在是否允许群员邀请好友加群
        /// </summary>
        public bool Current { get; set; }

        /// <summary>
        /// 允许群员邀请好友加群状态改变的群信息
        /// </summary>
        public Group Group { get; set; } = new();

        /// <summary>
        /// 操作的管理员或群主信息，当null时为Bot操作
        /// </summary>
        public Member? Operator { get; set; }
    }
}
