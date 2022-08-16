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
        public WeiboSearchData data { get; set; } = null!;
    }

    public class WeiboSearchData
    {
        public WeiboCardListInfo cardlistInfo { get; set; } = null!;
        public WeiboCard[] cards { get; set; } = Array.Empty<WeiboCard>();
    }

    public class WeiboCardListInfo
    {
        public string cardlist_title { get; set; } = string.Empty;

        public string desc { get; set; } = string.Empty;
    }

    public class WeiboCard
    {
        public int card_type { get; set; }

        public WeiboBlog mblog { get; set; } = null!;

    }

    public class WeiboBlog
    {
        public string text { get; set; } = null!;

        public string original_pic { get; set; } = null!;

        public WeiboUser user { get; set; } = null!;
    }

    public class WeiboUser
    {
        public string screen_name { get; set; } = string.Empty;
    }
}
