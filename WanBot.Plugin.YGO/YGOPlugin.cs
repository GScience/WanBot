using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;
using WanBot.Api.Util;
using WanBot.Graphic;
using WanBot.Plugin.Essential.Extension;
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

        private UIRenderer _renderer = null!;

        private string _databasePath = string.Empty;

        private CommandDispatcher _ygoCmdDispatcher = new();

        private Random _random = new();

        public override void PreInit()
        {
            YgoCardImage.CachePath = Path.Combine(GetConfigPath(), "cardPicCache");

            _databasePath = Path.Combine(GetConfigPath(), "cards.cdb");
            _ygoDatabase = new(Logger);
            _ygoDatabase.LoadAsync(_databasePath).Wait();

            _ygoCmdDispatcher["update"].Handle = async (s,o) => { await OnCommandUpdateDatabase(s, o); return true; };
            _ygoCmdDispatcher["search"].Handle = async (s, o) => { await OnCommandSearchCard(s, o); return true; };
            _ygoCmdDispatcher["searchadvance"].Handle = async (s, o) => { await OnCommandSearchAdvanceCard(s, o); return true; };
            _ygoCmdDispatcher["random"].Handle = async (s, o) => { await OnCommandRandom(s, o); return true; };

            base.PreInit();
        }

        public override void Start()
        {
            base.Start();

            this.GetBotHelp()
                .Category("游~戏~王")
                .Command("#查卡 关键词", "查找指定的卡片")
                .Command("#抽卡 关键词（可选）", "看看你和卡牌的羁绊")
                .Command("#高级查卡 检索命令", "超高级的卡片查询功能")
                .Command("#ygo update", "更新并重载卡牌数据库")
                .Info("胜利是属于我的！");

            _renderer = this.GetUIRenderer();
        }

        private async Task<int> DisplayCardAsync(MiraiBot bot, ISender sender, IEnumerable<YgoCard> cards, string search, int? reply = null, string prefix = "查找结果：")
        {
            if (!cards.Any())
            {
                await sender.ReplyAsync("未找到结果", reply);
                return 0;
            }

            var (image, count) = await CardRenderer.GenCardsImageAsync(_renderer, search, cards.GetEnumerator(), prefix);
            using var outputImage = new MiraiImage(bot, image);
            var builder = new MessageBuilder();
            builder.At(sender);
            builder.Image(outputImage);
            await sender.ReplyAsync(builder);
            return count;
        }

        [Command("ygo")]
        public async Task OnCommandYgo(MiraiBot bot, CommandEventArgs args)
        {
            await _ygoCmdDispatcher.HandleCommandAsync(bot, args);
        }

        public async Task OnCommandUpdateDatabase(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasPermission(this, "update"))
                return;

            await args.Sender.ReplyAsync($"正在重新同步卡牌数据库");
            await _ygoDatabase.UpdateAsync(_databasePath);
            args.Blocked = true;
        }

        [Command("抽卡")]
        public async Task OnCommandRandom(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasPermission(this, "random"))
                return;

            var codePlain = args.GetRemain()?.FirstOrDefault() as Plain;
            var filter = codePlain?.Text ?? "";

            try
            {
                var searchResult = _ygoDatabase.SearchByString(filter);
                var randomIndex = _random.Next(0, searchResult.Count);
                await DisplayCardAsync(bot, args.Sender, searchResult.Skip(randomIndex).Take(1), filter, args.GetMessageId(), "卡池：");
            }
            catch (ArgumentException e)
            {
                await args.Sender.ReplyAsync($"完犊子了！抽卡失败！不知道为啥参数出错了！");
                Logger.Error("Error while random card: {e}", e);
            }

            args.Blocked = true;
        }

        [Command("查卡")]
        public async Task OnCommandSearchCard(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasPermission(this, "search"))
                return;

            args.Blocked = true;

            var codePlain = args.GetRemain()?.FirstOrDefault() as Plain;
            var filter = codePlain?.Text ?? "";
            if (filter == "")
            {
                await args.Sender.ReplyAsync($"格式不正确，请输入 #查卡 卡片信息 来查卡");
                return;
            }

            try
            {
                var searchResult = _ygoDatabase.SearchByString(filter);
                var displayIndex = 0;
                do
                {
                    displayIndex += await DisplayCardAsync(bot, args.Sender, searchResult.Skip(displayIndex), filter, args.GetMessageId());
                } while (IsContinueDisplay(args.Sender));
            }
            catch (ArgumentException e)
            {
                await args.Sender.ReplyAsync($"完犊子了！查卡失败！不知道为啥参数出错了！");
                Logger.Error("Error while search card: {e}", e);
            }

            args.Blocked = true;
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
                    $"高级查询格式：#高级查卡 kw 青眼白龙 kd 魔法\n" +
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
                var searchResult = _ygoDatabase.SearchByYgoSearchCode(code);
                await DisplayCardAsync(bot, args.Sender, searchResult, code, args.GetMessageId());
            }
            catch (ArgumentException e)
            {
                await args.Sender.ReplyAsync($"完犊子了！查卡失败！参数是不是输错了！");
                Logger.Error("Error while search card: {e}", e);
            }

            args.Blocked = true;
        }

        /// <summary>
        /// 判断是否继续显示
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private bool IsContinueDisplay(ISender sender)
        {
            var chain = sender.WaitForReply(TimeSpan.FromSeconds(30));
            var str = chain?.AsPlain()?.Text;
            if (str != null && 
                (
                    str == "继续查" ||
                    str == "下一页" ||
                    str == "继续"
                ))
                return true;
            return false;
        }
    }
}