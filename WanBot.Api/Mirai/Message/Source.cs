using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// 消息源
    /// </summary>
    public class Source : BaseChain
    {
        /// <summary>
        /// 消息的识别号，用于引用回复（Source类型永远为chain的第一个元素）
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// 时间戳
        /// </summary>
        public int Time { get; set; } = 0;
    }
}
