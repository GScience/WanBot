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
        public long QQ { get; set; } = 123456789;

        /// <summary>
        /// host
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// 端口
        /// </summary>
        public ushort Port { get; set; } = 8080;

        /// <summary>
        /// 事件同步Id
        /// </summary>
        public string EventSyncId { get; set; } = "-1";

        /// <summary>
        /// 验证Key
        /// </summary>
        public string VerifyKey { get; set; } = "YourVerifyKey";
    }
}
