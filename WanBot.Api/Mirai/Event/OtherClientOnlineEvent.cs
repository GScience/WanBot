using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 其他客户端上线
    /// </summary>
    public class OtherClientOnlineEvent : BaseMiraiEvent
    {
        /// <summary>
        /// 其他客户端
        /// </summary>
        public Client Client { get; set; } = new();

        /// <summary>
        /// 详细设备类型
        /// </summary>
        public long? Kind { get; set; }
    }
}
