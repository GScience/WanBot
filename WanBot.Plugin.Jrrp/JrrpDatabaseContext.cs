using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.Jrrp
{
    internal class JrrpDatabaseContext : DbContext
    {
        public DbSet<JrrpUser> Users { get; set; } = null!;

        public string DbPath { get; }

        public JrrpDatabaseContext(string dbPath = "database.db")
        {
            DbPath = dbPath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
