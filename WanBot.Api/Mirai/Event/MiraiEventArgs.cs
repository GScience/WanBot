using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Mirai事件
    /// </summary>
    public class MiraiEventArgs
    {
        /// <summary>
        /// 是否取消事件的传递
        /// </summary>
        [JsonIgnore]
        public bool Cancel { get; set; }
    }
}
