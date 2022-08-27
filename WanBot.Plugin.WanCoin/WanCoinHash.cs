using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8618
namespace WanBot.Plugin.WanCoin
{
    /// <summary>
    /// 虚犊币Hash记录
    /// </summary>
    public class WanCoinHash
    {
        /// <summary>
        /// hash记录，采用long以便于查询
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Hash { get; internal set; }

        /// <summary>
        /// 字符串记录
        /// </summary>
        public string Str { get; set; } = string.Empty;

        /// <summary>
        /// 字符串所在群
        /// </summary>
        public long GroupId { get; set; }
    }
}
#pragma warning restore CS8618