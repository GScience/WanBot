using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 成员权限改变的事件（该成员不是Bot）
    /// </summary>
    public class MemberPermissionChangeEvent : BaseEvent
    {
        /// <summary>
        /// 原权限
        /// </summary>
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// 现权限
        /// </summary>
        public string Current { get; set; } = string.Empty;

        /// <summary>
        /// 权限改动的群员的信息
        /// </summary>
        public Member Member { get; set; } = new();
    }
}
