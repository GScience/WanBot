using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// Json消息
    /// </summary>
    public class Json : BaseChain
    {
        /// <summary>
        /// Json
        /// </summary>
        [JsonPropertyName("json")]
        public string Content { get; set; } = string.Empty;
    }
}
