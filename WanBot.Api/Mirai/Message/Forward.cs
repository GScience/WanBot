using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// 转发消息
    /// </summary>
    public class Forward : BaseChain
    {
        /// <summary>
        /// 消息节点
        /// </summary>
        public class Node
        {
            /// <summary>
            /// 发送人QQ号
            /// </summary>
            public long SenderId { get; set; }

            /// <summary>
            /// 发送时间
            /// </summary>
            public int Time { get; set; }

            /// <summary>
            /// 显示名称
            /// </summary>
            public string SenderName { get; set; } = string.Empty;

            /// <summary>
            /// 消息链
            /// </summary>
            public MessageChain? MessageChain { get; set; }

            /// <summary>
            /// 可以只使用消息messageId，从缓存中读取一条消息作为节点
            /// </summary>
            public int MessageId { get; set; }
        }

        /// <summary>
        /// 消息节点
        /// </summary>
        public List<Node>? NodeList { get; set; } = null;
    }
}
