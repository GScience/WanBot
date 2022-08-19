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
        private EssAttributeDatabaseContext _db;

        public EssAttrUserFactory(EssAttributeDatabaseContext db)
        {
            _db = db;
        }

        public EssAttrUser FromSender(ISender sender)
        {
            return new EssAttrUser(sender.Id, _db);
        }
    }
}
