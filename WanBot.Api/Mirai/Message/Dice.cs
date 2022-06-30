using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// 骰子
    /// </summary>
    public class Dice : BaseChain
    {
        /// <summary>
        /// 点数
        /// </summary>
        public int Value { get; set; }
    }
}
