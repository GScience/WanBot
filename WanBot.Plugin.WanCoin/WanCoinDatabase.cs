using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8618
namespace WanBot.Plugin.WanCoin
{
    /// <summary>
    /// 虚犊币数据库
    /// </summary>
    public class WanCoinDatabase : DbContext
    {
        /// <summary>
        /// 虚犊币用户
        /// </summary>
        public DbSet<WanCoinUser> Users { get; set; }

        /// <summary>
        /// 虚犊币Hash记录
        /// </summary>
        public DbSet<WanCoinHash> CoinHash { get; set; }

        /// <summary>
        /// 虚犊币交易记录
        /// </summary>
        public DbSet<WanCoinTrade> Trade { get; set; }

        public string DbPath { get; }

        public WanCoinDatabase(string dbPath = "database.db")
        {
            DbPath = dbPath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
#pragma warning restore CS8618
