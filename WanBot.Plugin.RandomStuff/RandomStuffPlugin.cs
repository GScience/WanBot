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

            var randNum = rand.Next(0, 8);
            var replacePersonalPronoun = true;

            switch (randNum)
            {
                // 1/20的概率发问号
                case 0:
                    await args.Sender.ReplyAsync("？");
                    return;

                // 1/20的概率发你说啥
                case 1:
                    await args.Sender.ReplyAsync("你说啥");
                    return;

                // 1/20的概率发嗷呜？
                case 2:
                    await args.Sender.ReplyAsync("嗷呜？");
                    return;

                // 1/20的概率颠倒复读
                case 3:
                    foreach (var msg in msgChain)
                        if (msg is Plain reversePlain)
                            reversePlain.Text = string.Concat(reversePlain.Text.Reverse());
                    break;

                // 1/20的概率不替换人称代词
                case 4:
                    replacePersonalPronoun = false;
                    break;
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

        [Command("随机对象")]
        public async Task OnRandomCp(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "cp"))
                return;

            if (args.Sender is not GroupSender groupSender)
                return;

            var groupMemberList = await bot.MemberListAsync(groupSender.GroupId);

            if (groupMemberList == null || groupMemberList.Data == null)
            {
                Logger.Error("Failed to get member list of group {groupId}", groupSender.GroupId);
                await args.Sender.ReplyAsync("完犊子了，不知道你群里都有谁");
                return;
            }

            var index = _random.Next(0, groupMemberList.Data.Count);
            var msgBuilder = new MessageBuilder();
            msgBuilder.At(groupSender).Text(" 你的对象是：").Text(groupMemberList.Data[index].MemberName);
            await groupSender.ReplyAsync(msgBuilder);

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
            msgBuilder.At(sender).ImageByUrl(jDocument["message"]);
            await sender.ReplyAsync(msgBuilder);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}