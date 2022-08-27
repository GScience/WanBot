using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// Xml消息
    /// </summary>
    public class Xml : BaseChain
    {
        /// <summary>
        /// Xml
        /// </summary>
        [JsonPropertyName("xml")]
        public string Content { get; set; } = string.Empty;

        public override int GetHashCode()
        {
            return Content.GetHashCode();
        }
    }
}
