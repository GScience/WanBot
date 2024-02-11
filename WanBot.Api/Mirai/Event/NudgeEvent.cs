using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 戳一戳事件
    /// </summary>
    public class NudgeEvent : BaseMiraiEvent
    {
        public class NugetSource
        {
            /// <summary>
            /// 来源的QQ号（好友）或群号
            /// </summary>
            public long Id { get; set; }

            /// <summary>
            /// 来源的类型，"Friend"或"Group"
            /// </summary>
            public string Kind { get; set; } = string.Empty;
        }

        /// <summary>
        /// 动作发出者的QQ号
        /// </summary>
        public long FromId { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public NugetSource Subject { get; set; } = new();

        /// <summary>
        /// 动作类型
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// 自定义动作内容
        /// </summary>
        public string Suffix { get; set; } = string.Empty;

        /// <summary>
        /// 动作目标的QQ号
        /// </summary>
        public long Target { get; set; }
    }
}
