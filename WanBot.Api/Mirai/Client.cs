using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai
{
    /// <summary>
    /// 客户端消息
    /// </summary>
    public class Client : ISender
    {
        /// <summary>
        /// 客户端Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 客户端平台
        /// </summary>
        public string Platform { get; set; } = string.Empty;
    }
}
