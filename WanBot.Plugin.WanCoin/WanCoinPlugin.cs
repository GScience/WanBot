using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Api.Util;
using WanBot.Plugin.Essential.EssAttribute;
using WanBot.Plugin.Essential.Extension;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.WanCoin
{
    public class WanCoinPlugin : WanBotPlugin
    {
        internal static DateTime StartTime = new DateTime(2022, 10, 1);

        internal const long ServerQQId = 1000;

        public override string PluginName => "WanCoin";

        public override string PluginDescription => "虚犊币插件，水的越多币越多~";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        /// <summary>
        /// Hash算法
        /// </summary>
        public Func<string, long, long> HashFunc { get; set; }
        private MD5 _md5 = MD5.Create();

        private EssAttrUserFactory _attrUsr = null!;

        /// <summary>
        /// 命令处理
        /// </summary>
        private CommandDispatcher _mainHandler = new();

        public WanCoinPlugin()
        {
            HashFunc = DefaultHashFunc;
        }

        private long DefaultHashFunc(string str, long groupId)
        {
            var salt = groupId == 0 ? "" : "WanCoin2.0${groupId}";
            var md5Byte = _md5.ComputeHash(Encoding.UTF8.GetBytes(salt + str));
            var hash = BitConverter.ToInt64(md5Byte);
            return hash;
        }

        private WanCoinDatabase GetWanCoinDatabase()
        {
            return new WanCoinDatabase(Path.Combine(GetConfigPath(), "wanCoin.db"));
        }

        public void ImportFrom(string filePath)
        {
            using var wanCoinDb = GetWanCoinDatabase();
            using var fileReader = File.OpenText(filePath);

            while (!fileReader.EndOfStream)
            {
                var line = fileReader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                var param = line.Split(";");

                // 第一个参数为行类型，c为虚犊币定义，u为用户定义
                var type = param[0];
                switch (type)
                {
                    case "c":
                        var str = param[2];
                        var groupId = 0;
                        wanCoinDb.CoinHash.Add(
                            new WanCoinHash
                            {
                                GroupId = groupId,
                                Hash = HashFunc(str, groupId),
                                Str = str
                            });
                        break;
                    case "u":
                        var usrId = long.Parse(param[1]);
                        var userCoins = long.Parse(param[2]);
                        var user = GetWanCoinUser(wanCoinDb, usrId);
                        user.CoinCount = userCoins;
                        break;
                }
                wanCoinDb.SaveChanges();
            }
        }

        public void ImportFromDatabase(string db)
        {
            using var wanCoinDb = GetWanCoinDatabase();
            using SqliteConnection connection = new SqliteConnection("Data Source=" + db);
            connection.Open();

            using var command =
                new SqliteCommand("SELECT * FROM Coin;", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var str = reader.GetString(1);
                var groupId = 0;
                wanCoinDb.CoinHash.Add(
                    new WanCoinHash
                    {
                        GroupId = groupId,
                        Hash = HashFunc(str, groupId),
                        Str = str
                    });
            }
            wanCoinDb.SaveChanges();
        }

        public override void PreInit()
        {
            using var wanCoinDb = GetWanCoinDatabase();
            wanCoinDb.Database.Migrate();
            wanCoinDb.SaveChanges();

            _mainHandler["买"].Handle = OnBuyCommand;
            _mainHandler["卖"].Handle = OnSellCommand;

            base.PreInit();
        }

        public override void Start()
        {
            this.GetBotHelp()
                .Category("虚犊币")
                .Command("#coin", "查看虚犊币信息")
                .Command("#coin 卖 数量", "卖出虚犊新币")
                .Command("#coin 买 数量", "买入虚犊新币")
                .Info("虚犊币是什么币呢？");

            _attrUsr = this.GetEssAttrUserFactory();
            base.Start();
        }

        /// <summary>
        /// 计算当前币价
        /// </summary>
        /// <param name="x">服务器拥有的币数</param>
        private static long GetCurrentPrise(long x)
        {
            // 根据银行的币数进行波动
            var y = 512.0 / (x * x / 1024.0 + 1) + 150 + 64 * (Math.Sin(x / 128.0) + Math.Sin(x / 1024.0));

            // 根据小时数进行波动，按照整小时计算
            var h = Math.Floor((DateTime.Now - StartTime).TotalHours);
            var k = h / 16.0 + Math.Sin(h / 16.0) * 16 + Math.Sin(h / 512.0) * 128;

            // 计算币价
            var prise = y * k;

            // 防止币价过小或过大
            if (prise < 10)
                prise = 10;
            else if (prise > 100000)
                prise = 100000;

            return (long)Math.Round(prise);
        }

        private WanCoinUser GetWanCoinUser(WanCoinDatabase wanCoinDb, long id)
        {
            var user = wanCoinDb.Users.Where(user => user.UserId == id).FirstOrDefault();
            if (user == null)
            {
                user = new WanCoinUser
                {
                    UserId = id
                };
                wanCoinDb.Users.Add(user);
                wanCoinDb.SaveChanges();
            }
            else
                wanCoinDb.CoinHash.Load();

            return user;
        }

        [Command("虚犊币", "coin")]
        public async Task OnWanCoinCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "coin"))
                return;

            args.Blocked = true;

            using var wanCoinDb = GetWanCoinDatabase();
            if (!await _mainHandler.HandleCommandAsync(bot, args))
            {
                var user = GetWanCoinUser(wanCoinDb, args.Sender.Id);
                var serverUser = GetWanCoinUser(wanCoinDb, ServerQQId);
                var sellPrise = GetCurrentPrise(serverUser.CoinCount);
                long? buyPrise = serverUser.CoinCount == 0 ? 
                    null : 
                    GetCurrentPrise(serverUser.CoinCount - 1);

                await args.Sender.ReplyAsync(
                    $"您有 {user.CoinCount} 枚虚犊新币\n" +
                    $"当前币价：卖({sellPrise}) 买({buyPrise?.ToString() ?? "没币了"})");
            }
        }

        /// <summary>
        /// 购买虚犊币
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> OnBuyCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "buy"))
                return false;

            // 获取买入数量
            var buyCount = 1;
            try
            {
                if (!int.TryParse(args.GetNextArgs<string>(), out buyCount) || 
                    buyCount <= 0)
                {
                    await args.Sender.ReplyAsync($"你要买入的数量好奇怪");
                    return true;
                }
            }
            catch (InvalidOperationException)
            {
            }

            using var wanCoinDb = GetWanCoinDatabase();
            var serverUser = GetWanCoinUser(wanCoinDb, ServerQQId);

            if (serverUser.CoinCount < buyCount)
                await args.Sender.ReplyAsync($"完犊子了，服务器的币只有{serverUser.CoinCount}了");
            else
            {
                // 记录是否购买成功
                var bought = false;
                long totalBuyPrise = 0;

                lock (this)
                {
                    // 计算需要花多少钱
                    for (var i = 0; i < buyCount; ++i)
                    {
                        var buyPrise = GetCurrentPrise(serverUser.CoinCount - 1 - i);
                        totalBuyPrise += buyPrise;
                    }

                    // 钱够吗
                    using var attrUsr = _attrUsr.FromSender(args.Sender);

                    if (attrUsr.Money < totalBuyPrise)
                        bought = false;
                    else
                    {
                        var user = GetWanCoinUser(wanCoinDb, args.Sender.Id);
                        attrUsr.Money -= totalBuyPrise;
                        serverUser.CoinCount -= buyCount;
                        user.CoinCount += buyCount;
                        wanCoinDb.SaveChanges();
                        bought = true;
                    }
                }

                if (bought)
                    await args.Sender.ReplyAsync($"你买了{buyCount}枚虚犊币，总共花了{totalBuyPrise}钱");
                else
                    await args.Sender.ReplyAsync($"钱不够了！需要{totalBuyPrise}钱才能买{buyCount}枚币！");
            }

            return true;
        }

        /// <summary>
        /// 卖出虚犊币
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<bool> OnSellCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "sell"))
                return false;

            // 获取卖出数量
            var sellCount = 1;
            try
            {
                if (!int.TryParse(args.GetNextArgs<string>(), out sellCount) || 
                    sellCount <= 0)
                {
                    await args.Sender.ReplyAsync($"你要卖出的数量好奇怪");
                    return true;
                }
            }
            catch (InvalidOperationException) 
            {
            }

            using var wanCoinDb = GetWanCoinDatabase();
            var serverUser = GetWanCoinUser(wanCoinDb, ServerQQId);
            var user = GetWanCoinUser(wanCoinDb, args.Sender.Id);

            if (user.CoinCount < sellCount)
                await args.Sender.ReplyAsync($"完犊子了，你的币只有{user.CoinCount}了");
            else
            {
                long totalSellPrise = 0;

                lock (this)
                {
                    // 计算卖了多少钱
                    for (var i = 0; i < sellCount; ++i)
                    {
                        var sellPrise = GetCurrentPrise(serverUser.CoinCount + i);
                        totalSellPrise += sellPrise;
                    }

                    using (var attrUsr = _attrUsr.FromSender(args.Sender))
                        attrUsr.Money += totalSellPrise;
                    user.CoinCount -= sellCount;
                    serverUser.CoinCount += sellCount;
                    wanCoinDb.SaveChanges();
                }
                await args.Sender.ReplyAsync($"你卖了{sellCount}枚虚犊币，赚了{totalSellPrise}钱！");
            }

            return true;
        }
    }
}