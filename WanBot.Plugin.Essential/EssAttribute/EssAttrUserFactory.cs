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

        public EssAttrUserFactory(string dbPath)
        {
            _dbPath = dbPath;
        }

        public EssAttrUser FromSender(ISender sender)
        {
            var db = new EssAttributeDatabaseContext(_dbPath);
            return new EssAttrUser(sender.Id, db);
        }
    }
}
