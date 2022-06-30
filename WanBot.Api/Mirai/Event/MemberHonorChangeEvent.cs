using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 群员称号改变
    /// </summary>
    public class MemberHonorChangeEvent : BaseEvent
    {
        /// <summary>
        /// 称号发生变化的群员的信息
        /// </summary>
        public Member Member { get; set; } = new();

        /// <summary>
        /// 称号变化行为：achieve获得称号，lose失去称号
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// 称号名称
        /// </summary>
        public string Honor { get; set; } = string.Empty;
    }
}
