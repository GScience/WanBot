using HtmlAgilityPack;
using System.Text;
using System.Text.Json;
using System.Web;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Plugin.EssentialPermission;

namespace WanBot.Plugin.HotSpot
{
    public class HotSpot : WanBotPlugin
    {
        public override string PluginName => "HotSpot";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private List<string> _searchCache = new();
        private string _hotspotCache = string.Empty;
        private DateTime _cacheTime = DateTime.MinValue;

        [Command("今日热点")]
        public async Task OnHotSpotCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!commandEvent.Sender.HasCommandPermission(this, "今日热点"))
                return;

            var topic = await GetHotSpotAsync();
            await commandEvent.Sender.ReplyAsync(topic); 
            commandEvent.Blocked = true;
        }

        public async Task<string> SearchAndGetTop(string keyword)
        {
            var web = new HtmlWeb();
            var containerId = HttpUtility.UrlEncode($"100103type=1&t=10&q={keyword}", Encoding.UTF8);
            var url = $"https://m.weibo.cn/api/container/getIndex?containerid={containerId}&page_type=searchall";
            var hotHtmlDoc = await web.LoadFromWebAsync(url);
            var text = hotHtmlDoc.DocumentNode.InnerText;
            var searchResult = JsonSerializer.Deserialize<WeiboSearchResult>(text);

            var blog = searchResult!.data.cards.Where((card) => card.card_type == 9).FirstOrDefault();

            return
                $"【{searchResult.data.cardlistInfo.cardlist_title}】{searchResult.data.cardlistInfo.desc}\n\n" +
                $"{blog!.mblog.user.screen_name} 表示：{blog.mblog.text}";
        }

        public async Task<string> GetHotSpotAsync()
        {
            var web = new HtmlWeb();
            var topicHtmlDoc = await web.LoadFromWebAsync("https://weibo.cn/pub/");
            var topics = topicHtmlDoc.DocumentNode.SelectNodes("/html/body//div[@class=\"c\"]");

            _searchCache.Clear();
            foreach (var topic in topics)
                _searchCache.Add(topic.InnerText);

            _hotspotCache =
                $"{await SearchAndGetTop(_searchCache[0])}\n" +
                $"{_searchCache[1]}\n" +
                $"{_searchCache[2]}\n" +
                $"{_searchCache[3]}\n" +
                "...";

            return _hotspotCache;
        }
    }
}