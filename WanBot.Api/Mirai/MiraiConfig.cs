using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai
{
    /// <summary>
    /// Mirai配置
    /// </summary>
    public class MiraiConfig
    {
        /// <summary>
        /// 绑定的QQ号
        /// </summary>
        public long QQ { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 验证Key
        /// </summary>
        public string VerifyKey { get; set; } = string.Empty;
    }
}
