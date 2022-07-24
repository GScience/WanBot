using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 同步群临时消息
    /// </summary>
    public class TempSyncMessage : BaseMiraiEvent
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public Member Subject { get; set; } = new();

        /// <summary>
        /// 消息链
        /// </summary>
        public MessageChain MessageChain { get; set; } = new();
    }
}
