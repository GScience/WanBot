using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Bot在群里的权限被改变. 操作人一定是群主
    /// </summary>
    public class BotGroupPermissionChangeEvent : BaseMiraiEvent
    {
        /// <summary>
        /// Bot的原权限，OWNER、ADMINISTRATOR或MEMBER
        /// </summary>
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// Bot的新权限，OWNER、ADMINISTRATOR或MEMBER
        /// </summary>
        public string Current { get; set; } = string.Empty;

        /// <summary>
        /// 权限改变所在的群信息
        /// </summary>
        public Group Group { get; set; } = new();
    }
}
