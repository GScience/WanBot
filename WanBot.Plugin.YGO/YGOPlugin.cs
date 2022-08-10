using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;
using WanBot.Graphic;
using WanBot.Plugin.Essential.Graphic;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.YGO
{
    public class YGOPlugin : WanBotPlugin
    {
        public override string PluginName => "YGO";

        public override string PluginAuthor => "WanNeng";

        public override string PluginDescription => "提供卡片的查询功能，后续会加入更多复杂的功能";
        public override Version PluginVersion => Version.Parse("1.0.0");

        private YgoDatabase _ygoDatabase = null!;

        private UIRenderer _renderer;

        public override void PreInit()
        {
            YgoCardImage.CachePath = Path.Combine(GetConfigPath(), "cardPicCache");

            _ygoDatabase = new(Logger);
            _ygoDatabase.LoadAsync(Path.Combine(GetConfigPath(), "cards.cdb")).Wait();
            base.PreInit();
        }

        public override void Start()
        {
            base.Start();

            _renderer = 
                Application.PluginManager.GetPlugin<GraphicPlugin>()?.Renderer 
                ?? throw new Exception("Failed to get renderer");
        }

        private async Task DisplayCardAsync(MiraiBot bot, ISender sender, List<YgoCard> cards, string search, int? reply = null, int max = 5)
        {
            if (cards.Count == 0)
            {
                await sender.ReplyAsync("未找到结果", reply);
                return;
            }

            var outputImage = await CardRenderer.GenCardsImageAsync(_renderer, search, cards);
            var builder = new MessageBuilder();
            builder.Image(new MiraiImage(bot, outputImage));
            await sender.ReplyAsync(builder);
        }

        [Command("查卡")]
        public async Task OnCommandSearchCard(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasPermission(this, "search"))
                return;

            args.Blocked = true;

            var codePlain = args.GetRemain()?.FirstOrDefault() as Plain;
            var keyword = codePlain?.Text ?? "";
            if (keyword == "")
            {
                await args.Sender.ReplyAsync($"格式不正确，请输入 .查卡 关键词 来查卡");
                return;
            }

            try
            {
                var searchResult = _ygoDatabase.SearchByKeyword(keyword);
                await DisplayCardAsync(bot, args.Sender, searchResult, keyword, args.GetMessageId());
            }
            catch (ArgumentException)
            {
                await args.Sender.ReplyAsync($"完犊子了！查卡失败！不知道为啥参数出错了！");
            }

            return;
        }

        [Command("高级查卡")]
        public async Task OnCommandSearchAdvanceCard(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasPermission(this, "searchadvance"))
                return;

            args.Blocked = true;

            var codePlain = args.GetRemain()?.FirstOrDefault() as Plain;
            var code = codePlain?.Text ?? "";
            if (code == "")
            {
                await args.Sender.ReplyAsync(
                    $"高级查询格式：.高级查卡 kw 青眼白龙 kd 魔法\n" +
                    $"kw(keyword) 检索关键词\n" +
                    $"kd(kind) 卡种，支持怪兽，魔法，陷阱\n" +
                    $"tp(type) 类型，支持XYZ等\n" +
                    $"ot OT\n" +
                    $"lv(level) 星/阶级，如1-4，也可以输入数值\n" +
                    $"sc(scale) 刻度，如1-4，也可以输入数值\n" +
                    $"atk(attack) 攻击，如1500-2000，也可以输入数值\n" +
                    $"def(defence) 防御，如1500-2000，也可以输入数值\n" +
                    $"att(attribute) 属性，如光地黑水炎神风，支持多个汉字组合\n" +
                    $"ra(race) 种族，如恶魔，多属性需要使用分割符分离");
                return;
            }
            Logger.Info($"Try to search card by search code: {code}");
            try
            {
                var searchResult = _ygoDatabase.SearchByCode(code);
                await DisplayCardAsync(bot, args.Sender, searchResult, code, args.GetMessageId());
            }
            catch (ArgumentException ex)
            {
                await args.Sender.ReplyAsync($"完犊子了！查卡失败！参数是不是输错了！");
                Logger.Info($"{code} throw an exception: {ex.Message}");
            }

            return;
        }
    }
}