using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;
using WanBot.Api.Util;
using WanBot.Plugin.Essential.EssAttribute;
using WanBot.Plugin.Essential.Extension;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.WanCoin
{
    public class WanCoinPlugin : WanBotPlugin
    {
        internal static DateTime StartTime = new DateTime(2021, 10, 1);

        internal const long ServerQQId = 1000;
        internal const ulong CoinHashMask = 0xFF;
        internal const ulong CoinHashEnd  = 0x69;

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

        /// <summary>
        /// 总币数
        /// </summary>
        private int _totalCoinCount = 0;

        private JrrpAddition? _jrrpAddition;

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

        internal WanCoinDatabase GetWanCoinDatabase()
        {
            return new WanCoinDatabase(Path.Combine(GetConfigPath(), "wanCoin.db"));
        }

        public void ImportFrom(string filePath)
        {
            using var wanCoinDb = GetWanCoinDatabase();
            using var fileReader = System.IO.File.OpenText(filePath);

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
            _totalCoinCount = wanCoinDb.CoinHash.Count();

            Logger.Info($"Total {_totalCoinCount} WanCoins");

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

            try
            {
                _jrrpAddition = new JrrpAddition(this);
            }
            catch (Exception e)
            {
                Logger.Warn("Failed to load JrrpAddition {e}", e);
            }

            _attrUsr = this.GetEssAttrUserFactory();
            base.Start();
        }

        /// <summary>
        /// 计算当前币价
        /// </summary>
        /// <param name="x"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private long GetCurrentPrice(long x, double d)
        {
            // x以10为阶梯对币价产生影响
            x = x / 10 * 10;

            // x不允许非正
            if (x<=0)
                x=1;

            // 虚犊币基础价格
            var c1 = 300;
            // 服务器所拥有的虚犊币占比
            var k1 = (double)x / _totalCoinCount;
            // 通货膨胀率（日）
            var k2 = 0.01;
            // 虚犊币膨胀后的价格
            var r1 = c1 + c1 * d * k2;

            // 完犊子希望持有20%的币，币少了就印钞收币，币多了就降价不收
            var k3 = 0.2 / k1;
            var r2 = r1 * k3;

            return (long)r2;
        }

        /// <summary>
        /// 计算当前币价
        /// </summary>
        /// <param name="x">服务器拥有的币数</param>
        internal long GetCurrentPrice(long x)
        {            
            // 根据天数进行波动，按照整小时进行离散运算
            var d = Math.Floor((DateTime.Now - StartTime).TotalHours) / 24;
            return GetCurrentPrice(x, d);
        }

        internal WanCoinUser GetWanCoinUser(WanCoinDatabase wanCoinDb, long id)
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
                var sellPrice = GetCurrentPrice(serverUser.CoinCount);
                long? buyPrice = serverUser.CoinCount == 0 ?
                    null :
                    GetCurrentPrice(serverUser.CoinCount - 1);

                await args.Sender.ReplyAsImageAsync(
                    $"您有 {user.CoinCount} 枚虚犊新币\n" +
                    $"当前币价：卖({sellPrice}) 买({buyPrice?.ToString() ?? "没币了"})", args.GetMessageId());
            }
        }

        /// <summary>
        /// 加入交易记录
        /// </summary>
        /// <param name="db"></param>
        /// <param name="user"></param>
        /// <param name="group"></param>
        /// <param name="count"></param>
        /// <param name="price"></param>
        /// <param name="time"></param>
        /// <param name="totalCount"></param>
        /// <param name="isBuy"></param>
        private void AddCoinTrade(
            WanCoinDatabase db,
            long user, 
            long group,
            long count,
            BigInteger price,
            DateTime time, 
            long totalCount, 
            bool isBuy)
        {
            db.Trade.Add(new WanCoinTrade
            {
                CoinCount = count,
                Group = group,
                IsBuy = isBuy,
                Time = time,
                Price = price.ToString(),
                TotalCount = totalCount,
                User = user
            });

            Logger.Info($"User {user} {(isBuy?"buy":"sell")} {count} coin(s) from group {group}");
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
                    await args.Sender.ReplyAsImageAsync($"你要买入的数量好奇怪", args.GetMessageId());
                    return true;
                }
            }
            catch (InvalidOperationException)
            {
            }

            using var wanCoinDb = GetWanCoinDatabase();
            var serverUser = GetWanCoinUser(wanCoinDb, ServerQQId);

            if (serverUser.CoinCount < buyCount)
                await args.Sender.ReplyAsImageAsync($"完犊子了，服务器的币只有{serverUser.CoinCount}了", args.GetMessageId());
            else
            {
                // 记录是否购买成功
                var bought = false;
                BigInteger totalBuyPrice = 0;

                lock (this)
                {
                    // 计算需要花多少钱
                    for (var i = 0; i < buyCount; ++i)
                    {
                        var buyPrice = GetCurrentPrice(serverUser.CoinCount - 1 - i);
                        totalBuyPrice += buyPrice;
                    }

                    // 钱够吗
                    using var attrUsr = _attrUsr.FromSender(args.Sender);

                    if (attrUsr.Money < totalBuyPrice)
                        bought = false;
                    else
                    {
                        var user = GetWanCoinUser(wanCoinDb, args.Sender.Id);
                        attrUsr.Money -= totalBuyPrice;
                        serverUser.CoinCount -= buyCount;
                        user.CoinCount += buyCount;

                        // 记录交易
                        long groupId = 0;
                        if (args.Sender is GroupSender gs)
                            groupId = gs.GroupId;
                        AddCoinTrade(
                            wanCoinDb,
                            args.Sender.Id,
                            groupId,
                            buyCount,
                            totalBuyPrice,
                            DateTime.Now,
                            serverUser.CoinCount,
                            true);

                        wanCoinDb.SaveChanges();
                        bought = true;
                    }
                }

                if (bought)
                    await args.Sender.ReplyAsImageAsync($"你买了{buyCount}枚虚犊币，总共花了{totalBuyPrice}钱，均价{totalBuyPrice / buyCount}", args.GetMessageId());
                else
                    await args.Sender.ReplyAsImageAsync($"钱不够了！需要{totalBuyPrice}钱才能买{buyCount}枚币！", args.GetMessageId());
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
                    await args.Sender.ReplyAsImageAsync($"你要卖出的数量好奇怪", args.GetMessageId());
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
                await args.Sender.ReplyAsImageAsync($"完犊子了，你的币只有{user.CoinCount}了");
            else
            {
                BigInteger totalSellPrice = 0;

                lock (this)
                {
                    // 计算卖了多少钱
                    for (var i = 0; i < sellCount; ++i)
                    {
                        var sellPrice = GetCurrentPrice(serverUser.CoinCount + i);
                        totalSellPrice += sellPrice;
                    }

                    using (var attrUsr = _attrUsr.FromSender(args.Sender))
                        attrUsr.Money += totalSellPrice;
                    user.CoinCount -= sellCount;
                    serverUser.CoinCount += sellCount;

                    // 记录交易
                    long groupId = 0;
                    if (args.Sender is GroupSender gs)
                        groupId = gs.GroupId;
                    AddCoinTrade(
                        wanCoinDb,
                        args.Sender.Id,
                        groupId,
                        sellCount,
                        totalSellPrice,
                        DateTime.Now,
                        serverUser.CoinCount,
                        false);

                    wanCoinDb.SaveChanges();
                }
                await args.Sender.ReplyAsImageAsync($"你卖了{sellCount}枚虚犊币，赚了{totalSellPrice}钱！均价{totalSellPrice / sellCount}", args.GetMessageId());
            }

            return true;
        }

        /// <summary>
        /// 挖矿
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        [MiraiEvent<GroupMessage>(Priority.Lowest)]
        public async Task OnGroupMessage(MiraiBot bot, GroupMessage e)
        {
            // 只获取纯文本消息来计算hash
            string? msg = null;
            Source? source = null;
            foreach (var chain in e.MessageChain)
            {
                if (chain is Source s)
                {
                    source = s;
                    continue;
                }
                else if (chain is Plain plain)
                {
                    if (msg != null)
                        return;

                    msg = plain.Text;
                }
                else
                    return;
            }
            if (msg == null)
                return;

            // 文本长度必须大于5小于40才属于有效消息
            if (msg.Length < 5 || msg.Length > 40)
                return;

            var hash = HashFunc(msg, e.Sender.Group.Id);

            // 是否满足虚犊币
            if (((ulong)hash & CoinHashMask) != CoinHashEnd)
                return;

            using var wanCoinDb = GetWanCoinDatabase();

            // 数据库中是否已经存在
            if (wanCoinDb.CoinHash.Where((WanCoinHash item) => item.Hash == hash).Any())
                return;

            // 添加虚犊币
            lock (this)
            {
                // 加币
                wanCoinDb.CoinHash.Add(
                    new WanCoinHash
                    {
                        GroupId = e.Sender.Group.Id,
                        Hash = hash,
                        Str = msg
                    });
                var user = GetWanCoinUser(wanCoinDb, e.Sender.Id);
                ++user.CoinCount;
                wanCoinDb.SaveChanges();
                ++_totalCoinCount;
            }

            Logger.Info($"New WanCoin found in group {e.Sender.Group.Id} with hash {hash}\n({msg})");

            if (source != null)
                await bot.SendGroupMessageAsync(e.Sender.Group.Id, source.Id, "好，给你一枚虚犊币吧~");
            else
                await bot.SendGroupMessageAsync(e.Sender.Group.Id, null, 
                    $"刚才好像听见有人说：{msg}" +
                    $"\n感觉这串文字有着特殊的含义，奖励一枚虚犊币");
        }
    }
}
