using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// 文本消息
    /// </summary>
    public class Plain : BaseChain
    {
        /// <summary>
        /// 文字消息
        /// </summary>
        public string Text { get; set; } = string.Empty;
    }
}
