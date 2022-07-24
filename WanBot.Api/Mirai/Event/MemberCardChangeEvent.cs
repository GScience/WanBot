using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 群名片改动
    /// </summary>
    public class MemberCardChangeEvent : BaseMiraiEvent
    {
        /// <summary>
        /// 原本名片
        /// </summary>
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// 现在名片
        /// </summary>
        public string Current { get; set; } = string.Empty;

        /// <summary>
        /// 名片改动的群员的信息
        /// </summary>
        public Member Member { get; set; } = new();
    }
}
