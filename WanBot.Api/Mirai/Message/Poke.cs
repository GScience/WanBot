using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// 戳一戳消息
    /// </summary>
    public class Poke : BaseChain
    {
        /// <summary>
        /// 戳一戳的类型
        /// </summary>
        public string Name { get; set; } = string.Empty;

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
