using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// 表情消息
    /// </summary>
    public class Face : BaseChain
    {
        /// <summary>
        /// QQ表情编号，可选，优先高于name
        /// </summary>
        public int FaceId { get; set; }

        /// <summary>
        /// QQ表情拼音，可选
        /// </summary>
        public string? Name { get; set; } = null;

        public override int GetHashCode()
        {
            return FaceId.GetHashCode();
        }
    }
}
