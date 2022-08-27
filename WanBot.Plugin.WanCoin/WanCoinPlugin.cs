using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Api.Util;
using WanBot.Plugin.Essential.Extension;

namespace WanBot.Plugin.WanCoin
{
    public class WanCoinPlugin : WanBotPlugin
    {
        internal const long ServerQQId = 1000;

        public override string PluginName => "WanCoin";

        public override string PluginDescription => "虚犊币插件，水的越多币越多~";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        /// <summary>
        /// Hash算法
        /// </summary>
        public Func<string, long, long> HashFunc { get; set; }

        private CommandDispatcher _mainHandler = new();
        private MD5 _md5 = MD5.Create();

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
                .Command("#coin", "查看虚犊币信息")
                .Command("#coin 卖 数量", "卖出虚犊新币")
                .Command("#coin 买 数量", "买入虚犊新币");
            base.Start();
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
            args.Blocked = true;

            using var wanCoinDb = GetWanCoinDatabase();
            if (!await _mainHandler.HandleCommandAsync(bot, args))
            {
                var user = GetWanCoinUser(wanCoinDb, args.Sender.Id);
                await args.Sender.ReplyAsync($"您有 {user.CoinCount} 枚虚犊新币");
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
            using var wanCoinDb = GetWanCoinDatabase();
            var serverUser = GetWanCoinUser(wanCoinDb, ServerQQId);

            if (serverUser.CoinCount == 0)
                await args.Sender.ReplyAsync($"完犊子了，服务器没币了");
            else
            {
                lock (this)
                {
                    var user = GetWanCoinUser(wanCoinDb, args.Sender.Id);
                    --serverUser.CoinCount;
                    ++user.CoinCount;
                    wanCoinDb.SaveChanges();
                }
                await args.Sender.ReplyAsync($"你买了一枚虚犊币");
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
            using var wanCoinDb = GetWanCoinDatabase();
            var serverUser = GetWanCoinUser(wanCoinDb, ServerQQId);
            var user = GetWanCoinUser(wanCoinDb, args.Sender.Id);

            if (user.CoinCount == 0)
                await args.Sender.ReplyAsync($"完犊子了，你没币了");
            else
            {
                lock (this)
                {
                    --user.CoinCount;
                    ++serverUser.CoinCount;
                    wanCoinDb.SaveChanges();
                }
                await args.Sender.ReplyAsync($"你卖了一枚虚犊币");
            }

            return true;
        }
    }
}