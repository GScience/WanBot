using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 群头衔改动（只有群主有操作限权）
    /// </summary>
    public class MemberSpecialTitleChangeEvent : BaseMiraiEvent
    {
        /// <summary>
        /// 原头衔
        /// </summary>
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// 现头衔
        /// </summary>
        public string Current { get; set; } = string.Empty;

        /// <summary>
        /// 头衔改动的群员的信息
        /// </summary>
        public Member Member { get; set; } = new();
    }
}
