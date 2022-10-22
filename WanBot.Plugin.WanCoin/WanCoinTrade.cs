using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.WanCoin
{
    /// <summary>
    /// 记录虚犊币交易
    /// </summary>
    public class WanCoinTrade
    {
        /// <summary>
        /// 交易Id
        /// </summary>
        public long Id { get; internal set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public long User { get; set; }

        /// <summary>
        /// 交易所在的群
        /// </summary>
        public long Group { get; set; }
        
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public string Price { get; set; } = "0";

        /// <summary>
        /// 币数
        /// </summary>
        public long CoinCount { get; set; }

        /// <summary>
        /// 服务器总币数
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 是否为购买
        /// </summary>
        public bool IsBuy { get; set; }
    }
}
