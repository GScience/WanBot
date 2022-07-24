using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 可取消的事件
    /// </summary>
    public class CancellableEventArgs
    {
        /// <summary>
        /// 是否取消事件的传递
        /// </summary>
        [JsonIgnore]
        public bool Cancel { get; set; }
    }
}
