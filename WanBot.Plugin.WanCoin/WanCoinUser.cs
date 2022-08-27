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
    /// 虚犊币用户
    /// </summary>
    public class WanCoinUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long UserId { get; internal set; }

        /// <summary>
        /// 币数
        /// </summary>
        public long CoinCount { get; set; }
    }
}
#pragma warning restore CS8618