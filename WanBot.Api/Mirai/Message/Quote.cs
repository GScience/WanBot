using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// 引用回复消息
    /// </summary>
    public class Quote : BaseChain
    {
        /// <summary>
        /// 被引用回复的原消息的messageId
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// 被引用回复的原消息所接收的群号，当为好友消息时为0
        /// </summary>
        public long GroupId { get; set; } = 0;

        /// <summary>
        /// 被引用回复的原消息的发送者的QQ号
        /// </summary>
        public long SenderId { get; set; } = 0;

        /// <summary>
        /// 被引用回复的原消息的接收者者的QQ号（或群号）
        /// </summary>
        public long TargetId { get; set; } = 0;

        /// <summary>
        /// 被引用回复的原消息的消息链对象
        /// </summary>
        public MessageChain? Origin { get; set; } = null;

        public override int GetHashCode()
        {
            return
                Id.GetHashCode() ^
                GroupId.GetHashCode() ^
                SenderId.GetHashCode() ^
                TargetId.GetHashCode() ^
                (Origin?.GetHashCode() ?? 0);
        }
    }
}
