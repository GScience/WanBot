using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.Essential.EssAttribute
{
    public class EssAttributeDatabaseContext : DbContext
    {
        public DbSet<DbEssAttributeUser> Users { get; set; } = null!;

        public string DbPath { get; }

        public EssAttributeDatabaseContext(string dbPath = "database.db")
        {
            DbPath = dbPath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
