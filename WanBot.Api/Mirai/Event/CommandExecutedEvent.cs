using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 命令被执行
    /// </summary>
    public class CommandExecutedEvent : BaseMiraiEvent
    {
        /// <summary>
        /// 其他客户端
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 发送命令的好友, 从控制台发送为 null
        /// </summary>
        public Friend? Friend { get; set; }

        /// <summary>
        /// 发送命令的好友, 从控制台发送为 null
        /// </summary>
        public Member? Member { get; set; }

        /// <summary>
        /// 指令的参数, 以消息类型传递
        /// </summary>
        public MessageChain? Args { get; set; }
    }
}
