using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 坦白说
    /// </summary>
    public class GroupAllowConfessTalkEvent : BaseMiraiEvent
    {
        /// <summary>
        /// 原本坦白说是否开启
        /// </summary>
        public bool Origin { get; set; }

        /// <summary>
        /// 现在坦白说是否开启
        /// </summary>
        public bool Current { get; set; }

        /// <summary>
        /// 坦白说状态改变的群信息
        /// </summary>
        public Group Group { get; set; } = new();

        /// <summary>
        /// 是否Bot进行该操作
        /// </summary>
        public bool IsByBot { get; set; }
    }
}
