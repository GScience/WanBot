using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// 商城表情消息，目前商城表情仅支持接收和转发，不支持构造发送
    /// </summary>
    public class MarketFace : BaseChain
    {
        /// <summary>
        /// 商城表情唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 表情显示名称
        /// </summary>
        public string? Name { get; set; } = null;
    }
}
