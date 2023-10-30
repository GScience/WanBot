using System.Net;
using System.Text.Json;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;
using WanBot.Graphic;
using WanBot.Graphic.Util;
using WanBot.Plugin.Essential.Extension;
using WanBot.Plugin.Essential.Permission;
using WanBot.Plugin.RandomStuff.Food;

namespace WanBot.Plugin.RandomStuff
{
    public class RandomStuffPlugin : WanBotPlugin , IDisposable
    {
        public override string PluginName => "RandomStuff";

        public override string PluginDescription => "生成一些与随即相关的有趣内容";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private Random _random = new();
        private HttpClient _httpClient = new();

        private UIRenderer _renderer = null!;

        private RandomStuffConfig _config = null!;

        private RandomFoodGenerator _foodGenerator = new();

        public override void PreInit()
        {
            _config = GetConfig<RandomStuffConfig>();
            base.PreInit();
        }

        public override void Start()
        {
            this.GetBotHelp()
                .Category("随机小功能")
                .Command("#随机对象", "抓个群里的人当对象（默认禁用）")
                .Command("#舔我", "找个舔狗舔你")
                .Command("#说", "完犊子是一只不听话的小鹦鹉")
                .Command("#来只狗", "看看可爱小狗狗")
                .Command("#来只猫", "看看可爱小猫猫")
                .Command("#来只狐狸", "看看可爱小狐狸")
                .Command("#吃什么", "看看今天吃啥")
                .Info("如果不如意可以打爆完犊子");

            _renderer = this.GetUIRenderer();
            base.Start();
        }

        [Command("说")]
        public async Task OnTalkCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "talk"))
                return;

            args.Blocked = true;

            var seed = args.Sender.Id;
            var msgChain = args.GetRemain()?.ToList();
            if (msgChain == null || msgChain.Count == 0)
            {
                await args.Sender.ReplyAsync("说啥？");
                return;
            }

            // 去掉第一个字符串前边的空格
            if (msgChain[0] is Plain firstPlain)
                firstPlain.Text = firstPlain.Text.TrimStart();

            var hashCode = 0;
            foreach (var msg in msgChain)
                hashCode ^= msg.GetHashCode();
            var rand = new Random(seed.GetHashCode() ^ hashCode);

            var randNum = rand.Next(0, _config.RateBase);
            var replacePersonalPronoun = true;

            switch (randNum)
            {
                // 1/20的概率发随机质疑词
                case 0:
                    var wordIndex = rand.Next(0, _config.TalkWords.Length);
                    await args.Sender.ReplyAsync(_config.TalkWords[wordIndex]);
                    return;

                // 1/20的概率颠倒复读
                case 1:
                    foreach (var msg in msgChain)
                        if (msg is Plain reversePlain)
                            reversePlain.Text = string.Concat(reversePlain.Text.Reverse());
                    break;

                // 1/20的概率不替换人称代词
                case 2:
                    replacePersonalPronoun = false;
                    break;

                // 1/20的概率只戳一戳
                case 3:
                    await args.Sender.NudgeAsync();
                    return;
            }

            // 替换人称
            if (replacePersonalPronoun)
                foreach (var msg in msgChain)
                    if (msg is Plain replacePlain)
                        replacePlain.Text = string.Concat(
                            replacePlain.Text.Select(
                                c => 
                                    c == '我' ? '你' :
                                    c == '你' ? '我' : 
                                    c
                                ));

            await args.Sender.ReplyAsync(new MessageChain(msgChain));
        }

        private List<double> GenerateMemberWeight(List<Member> members)
        {
            return members
                // get total days
                .Select(m => (m, d:(int)((DateTime.Now.Date - DateTimeOffset.FromUnixTimeSeconds(m.LastSpeakTimestamp)).TotalDays)))
                // get scaled days as weight_1
                .Select(pair => (pair.m, w:1.0 / Math.Max(1, pair.d)))
                // weight_1 + weight_2
                .Select(pair => pair.w + new Random((int)pair.m.Id).Next(0, 1))
                // weight is between 10 to 0.001
                .Select(w => Math.Min(10, Math.Max(0.001, w)))
                .ToList();
        }

        [Command("随机对象")]
        public async Task OnRandomCp(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "cp"))
                return;

            if (args.Sender is not GroupSender groupSender)
                return;
            var rand = new Random($"{args.Sender.Id}{DateTime.Now.Date:d}".GetHashCode());
            var groupMemberList = await bot.MemberListAsync(groupSender.GroupId);
            
            if (groupMemberList == null || groupMemberList.Data == null)
            {
                Logger.Error("Failed to get member list of group {groupId}", groupSender.GroupId);
                await args.Sender.ReplyAsync("完犊子了，不知道你群里都有谁");
                return;
            }
            var weights = GenerateMemberWeight(groupMemberList.Data);

            if (rand.Next(0, 5) == 1)
            {
                var msgBuilder = new MessageBuilder();
                msgBuilder.At(groupSender).Text(" 你今天没有对象");
                await groupSender.ReplyAsync(msgBuilder);
            }
            else
            {
                var totalWeight = weights.Sum();
                var index = -1;
                var currentWeight = 0.0;
                var randWeight = rand.NextDouble() * totalWeight;
                for (var i = 0; i < weights.Count; ++i)
                {
                    currentWeight += weights[i];
                    if (currentWeight > randWeight)
                    {
                        index = i;
                        break;
                    }
                }
                var msgBuilder = new MessageBuilder();
                if (index == -1)
                    msgBuilder.At(groupSender).Text(" 很奇怪，你本来应该有对象的");
                else
                {
                    var member = groupMemberList.Data[index];
                    var memberProfile = await bot.MemberProfileAsync(groupSender.GroupId, member.Id);
                    var memberDisplayName = 
                        string.IsNullOrEmpty(memberProfile.Nickname) ? 
                        groupMemberList.Data[index].MemberName : 
                        memberProfile.Nickname;
                    msgBuilder.At(groupSender).Text(" 你今天的对象是：").Text(memberDisplayName);
                }
                await groupSender.ReplyAsync(msgBuilder);
            }
            args.Blocked = true;
        }

        [Command("舔我")]
        public async Task OnLickDogCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "LickDog"))
                return;

            var msg = await _httpClient.GetStringAsync("https://api.ixiaowai.cn/tgrj/index.php");

            msg = string.Concat(
                msg.Select(
                    c =>
                        c == '他' ? '你' :
                        c == '她' ? '你' : 
                        c
                    ));

            var verticalHelper = new VerticalHelper();
            verticalHelper
                .Box($"    To {args.Sender.DisplayName}: \n    {msg}", 16, textAlignment: SkiaSharp.SKTextAlign.Left)
                .Width(500);
            using var layout = verticalHelper.VerticalLayout;
            using var miraiImage = new MiraiImage(bot, _renderer.Draw(layout));
            var msgBuilder = new MessageBuilder();
            msgBuilder.At(args.Sender).Image(miraiImage);
            await args.Sender.ReplyAsync(msgBuilder); 
            
            args.Blocked = true;
        }

        [Command("来只狗")]
        public async Task OnDogCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "dog"))
                return;

            args.Blocked = true;
            await SendRandomDog(args.Sender, true);
        }

        [Command("来只狐狸")]
        public async Task OnFoxCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "fox"))
                return;

            args.Blocked = true;
            await SendRandomFox(args.Sender, true);
        }

        [Command("来只猫")]
        public async Task OnCatCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "cat"))
                return;

            args.Blocked = true;
            await SendRandomCat(args.Sender, true);
        }

        [Command("吃什么")]
        public async Task OnEatCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "eat"))
                return;

            args.Blocked = true;
            await SendFood(args.Sender);
        }

        [Regex("狗")]
        public async Task OnDogKeyword(MiraiBot bot, RegexEventArgs args)
        {
            if (_random.Next(0, 30) != 5)
                return;

            if (!args.Sender.HasCommandPermission(this, "dog"))
                return;

            args.Blocked = true;
            await SendRandomDog(args.Sender, false);
        }

        [Regex("猫")]
        public async Task OnCatKeyword(MiraiBot bot, RegexEventArgs args)
        {
            if (_random.Next(0, 30) != 5)
                return;

            if (!args.Sender.HasCommandPermission(this, "cat"))
                return;

            args.Blocked = true;
            await SendRandomCat(args.Sender, false);
        }

        [Regex("狐狸")]
        public async Task OnFoxKeyword(MiraiBot bot, RegexEventArgs args)
        {
            if (_random.Next(0, 30) != 5)
                return;

            if (!args.Sender.HasCommandPermission(this, "fox"))
                return;

            await SendRandomFox(args.Sender, false);
        }

        [Regex("(吃(什么|啥)|好饿|饿了)")]
        public async Task OnEatCommand(MiraiBot bot, RegexEventArgs args)
        {
            if (_random.Next(0, 4) != 2)
                return;

            if (!args.Sender.HasCommandPermission(this, "eat"))
                return;

            args.Blocked = true;
            await SendFood(args.Sender);
        }

        /// <summary>
        /// 随机猫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="atSender"></param>
        /// <returns></returns>
        public async Task SendRandomCat(ISender sender, bool atSender)
        {
            if (_random.Next(0, 10) == 5)
            {
                // 网络状态猫
                var statusCodes = Enum.GetValues<HttpStatusCode>();
                var netState = (int)statusCodes[_random.Next(0, statusCodes.Length)];
                var msgBuilder = new MessageBuilder();
                if (atSender)
                    msgBuilder.At(sender);
                msgBuilder.ImageByUrl($"https://http.cat/{netState}");
                await sender.ReplyAsync(msgBuilder);
            }
            else
            {
                // 普通猫
                var catRequest = await _httpClient.GetStringAsync("https://api.thecatapi.com/v1/images/search");
                var jDocument = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(catRequest[1..^1])!;
                var msgBuilder = new MessageBuilder();
                if (atSender)
                    msgBuilder.At(sender);
                msgBuilder.ImageByUrl(jDocument["url"].GetString()!);
                await sender.ReplyAsync(msgBuilder);
            }
        }

        /// <summary>
        /// 随机狐狸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="atSender"></param>
        /// <returns></returns>
        public async Task SendRandomFox(ISender sender, bool atSender)
        {
            var foxRequest = await _httpClient.GetStringAsync("https://randomfox.ca/floof/");
            var jDocument = JsonSerializer.Deserialize<Dictionary<string, string>>(foxRequest)!;
            var msgBuilder = new MessageBuilder();
            if (atSender)
                msgBuilder.At(sender);
            msgBuilder.ImageByUrl(jDocument["image"]);
            await sender.ReplyAsync(msgBuilder);
        }

        /// <summary>
        /// 随机狗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="atSender"></param>
        /// <returns></returns>
        public async Task SendRandomDog(ISender sender, bool atSender)
        {
            var dogRequest = await _httpClient.GetStringAsync("https://dog.ceo/api/breeds/image/random");
            var jDocument = JsonSerializer.Deserialize<Dictionary<string, string>>(dogRequest)!;
            var msgBuilder = new MessageBuilder();
            if (atSender)
                msgBuilder.At(sender);
            msgBuilder.ImageByUrl(jDocument["message"]);
            await sender.ReplyAsync(msgBuilder);
        }

        /// <summary>
        /// 今天吃啥
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public async Task SendFood(ISender sender)
        {
            // 可能会没饭吃
            if (_random.Next(0, 5) == 1)
            {
                await sender.ReplyAsync($"没饭吃真惨");
                return;
            }

            string food;
            // 可能会吃群友
            if (_random.Next(0, 5) == 1 && sender is GroupSender groupSender)
            {
                var groupMemberList = await sender.Bot.MemberListAsync(groupSender.GroupId);

                if (groupMemberList == null || groupMemberList.Data == null)
                {
                    Logger.Error("Failed to get member list of group {groupId}", groupSender.GroupId);
                    await sender.ReplyAsync("你想吃群友但是没吃到");
                    return;
                }
                var index = _random.Next(0, groupMemberList.Data.Count);
                food = groupMemberList.Data[index].MemberName;
            }
            // 正常食物
            else
                food = _foodGenerator.GenFood();

            Logger.Info($"Send a food call {food}");
            await (_random.Next(0, 5) switch
            {
                0 => sender.ReplyAsync($"今天吃{food}吧"),
                1 => sender.ReplyAsync($"不如试试{food}"),
                2 => sender.ReplyAsync($"要不然去吃{food}？"),
                3 => sender.ReplyAsync($"{food}咋样"),
                4 => sender.ReplyAsync($"尝尝{food}"),
                _ => sender.ReplyAsync(food)
            });
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}