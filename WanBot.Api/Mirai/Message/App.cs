using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// App消息
    /// </summary>
    public class App : BaseChain
    {
        /// <summary>
        /// App
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }
}
