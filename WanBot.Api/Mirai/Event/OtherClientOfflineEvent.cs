using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 其他客户端下线
    /// </summary>
    public class OtherClientOfflineEvent : BaseMiraiEvent
    {
        /// <summary>
        /// 其他客户端
        /// </summary>
        public Client Client { get; set; } = new();
    }
}
