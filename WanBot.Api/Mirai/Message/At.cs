using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// At
    /// </summary>
    public class At : BaseChain
    {
        /// <summary>
        /// 群员QQ号
        /// </summary>
        public long Target { get; set; } = 0;

        /// <summary>
        /// At时显示的文字，发送消息时无效，自动使用群名片
        /// </summary>
        public string Display { get; set; } = string.Empty;
    }
}
