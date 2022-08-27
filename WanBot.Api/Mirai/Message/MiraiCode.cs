using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// Mirai Code
    /// </summary>
    public class MiraiCode : BaseChain
    {
        /// <summary>
        /// Mirai Code
        /// </summary>
        public string Code { get; set; } = string.Empty;

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
    }
}
