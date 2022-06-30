using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// 文件消息
    /// </summary>
    public class File : BaseChain
    {
        /// <summary>
        /// 文件识别id
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }
    }
}
