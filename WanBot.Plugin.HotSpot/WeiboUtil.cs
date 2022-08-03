using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.HotSpot
{
    public class WeiboSearchResult
    {
        public int ok { get; set; }
        public WeiboSearchData data { get; set; }
    }

    public class WeiboSearchData
    {
        public WeiboCardListInfo cardlistInfo { get; set; }
        public WeiboCard[] cards { get; set; }
    }

    public class WeiboCardListInfo
    {
        public string cardlist_title { get; set; }

        public string desc { get; set; }
    }

    public class WeiboCard
    {
        public int card_type { get; set; }

        public WeiboBlog mblog { get; set; }

    }

    public class WeiboBlog
    {
        public string text { get; set; }

        public WeiboUser user { get; set; }
    }

    public class WeiboUser
    {
        public string screen_name { get; set; }
    }
}
