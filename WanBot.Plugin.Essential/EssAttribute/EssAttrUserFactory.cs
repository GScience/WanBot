using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;

namespace WanBot.Plugin.Essential.EssAttribute
{
    public class EssAttrUserFactory
    {
        private string _dbPath;

        private EssAttributeDatabaseContext _db;

        public EssAttrUserFactory(string dbPath)
        {
            _dbPath = dbPath;
            _db = new EssAttributeDatabaseContext(_dbPath);
        }

        public EssAttrUser FromSender(ISender sender)
        {
            return new EssAttrUser(sender.Id, _db);
        }
    }
}
