using Microsoft.EntityFrameworkCore;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Plugin.Essential.Extension;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.Jrrp
{
    public class JrrpPlugin : WanBotPlugin, IDisposable
    {
        private readonly Random _random = new((int)DateTime.Now.Ticks);

        public override string PluginName => "Jrrp";

        public override string PluginAuthor => "WanNeng";

        public override string PluginDescription => "算命，但是不准，仅供娱乐";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private JrrpDatabaseContext GetDbContext()
        {
            return new JrrpDatabaseContext(Path.Combine(GetConfigPath(), "jrrp.db"));
        }

        private JrrpConfig _config = null!;

        public override void PreInit()
        {
            using var jrrpDb = GetDbContext();
            jrrpDb.Database.Migrate();
            jrrpDb.SaveChanges();

            _config = GetConfig<JrrpConfig>();
            base.PreInit();
        }

        public override void Start()
        {
            this.GetBotHelp()
                .Category("今日运势")
                .Command("#jrrp", "查看今日运势")
                .Command("#我应该 AAA还是BBB还是.....", "看看自己应该怎么办")
                .Info("准不准另说（");

            base.Start();
        }

        [Command("jrrp")]
        public async Task OnJrrp(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "jrrp"))
                return;

            args.Blocked = true;
            using var jrrpDb = GetDbContext();
            var jrrpUser = await GetJrrpUserAsync(jrrpDb, args.Sender.Id);
            if (jrrpUser.CanDo > _config.Activity.Count)
                jrrpUser.CanDo = 0;
            if (jrrpUser.CantDo > _config.Activity.Count)
                jrrpUser.CantDo = _config.Activity.Count - 1;

            await args.Sender.ReplyAsync(
                $"今日运势：{(int)(jrrpUser.Jrrp * 100)}\n\n" +
                $"     宜：{_config.Activity[jrrpUser.CanDo]}\n\n" +
                $" 不宜：{_config.Activity[jrrpUser.CantDo]}");
        }

        [Command("我应该")]
        public async Task OnWhatShouldIDo(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "WhatShouldIDo"))
                return;

            args.Blocked = true;

            try
            {
                var rawText = args.GetNextArgs<string>();
                var selections = rawText.Split("还是");

                // 检查是否应该回复完犊子
                if (_random.Next(0, 20) == 5)
                    await args.Sender.ReplyAsync($"完犊子了，你什么都不应该做");
                else
                {
                    if (selections.Length == 0)
                        await args.Sender.ReplyAsync($"?");
                    else if (selections.Length == 1)
                        await args.Sender.ReplyAsync($"你不应该 {selections[0]} 哦");
                    else
                    {
                        var rndIndex = _random.Next(0, selections.Length);
                        await args.Sender.ReplyAsync($"你应该 {selections[rndIndex]} 哦");
                    }
                }
            }
            catch (InvalidOperationException)
            {
                await args.Sender.ReplyAsync($"你应该好好检查自己的参数哦~");
            }
        }

        private async Task<JrrpUser> GetJrrpUserAsync(JrrpDatabaseContext jrrpDb, long id)
        {
            var user = await jrrpDb.Users.SingleOrDefaultAsync(jrrpUser => jrrpUser.Id == id);

            if (user == null)
            {
                user = new()
                {
                    Id = id,
                    LastTime = DateTime.MinValue,
                    Jrrp = 0
                };
                await jrrpDb.Users.AddAsync(user);
            }

            var now = DateTime.Now;
            if (now.Day != user.LastTime.Day)
            {
                user.LastTime = now;
                user.Jrrp = _random.NextSingle();
                user.CanDo = _random.Next(0, _config.Activity.Count);
                do
                {
                    user.CantDo = _random.Next(0, _config.Activity.Count);
                } while (user.CantDo == user.CanDo);
            }

            await jrrpDb.SaveChangesAsync();
            return user;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}