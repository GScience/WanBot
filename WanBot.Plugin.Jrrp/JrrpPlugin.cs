using Microsoft.EntityFrameworkCore;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;

namespace WanBot.Plugin.Jrrp
{
    public class JrrpPlugin : WanBotPlugin, IDisposable
    {
        private readonly Random _random = new((int)DateTime.Now.Ticks);

        public override string PluginName => "Jrrp";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");
        private JrrpDatabaseContext _jrrpDb = null!;

        public override void PreInit()
        {
            _jrrpDb = new JrrpDatabaseContext(Path.Combine(GetConfigPath(), "jrrp.db"));
            _jrrpDb.Database.Migrate();

            base.PreInit();
        }

        [Command("jrrp")]
        public async Task OnJrrp(MiraiBot bot, CommandEventArgs args)
        {
            var jrrpUser = await GetJrrpUserAsync(args.Sender.Id);
            await args.Sender.ReplyAsync($"今日运势：{jrrpUser.Jrrp}");
        }

        public async Task<JrrpUser> GetJrrpUserAsync(long id)
        {
            var user = await _jrrpDb.Users.SingleOrDefaultAsync(jrrpUser => jrrpUser.Id == id);

            if (user == null)
            {
                user = new()
                {
                    Id = id,
                    LastTime = DateTime.MinValue,
                    Jrrp = 0
                };
                await _jrrpDb.Users.AddAsync(user);
            }

            var now = DateTime.Now;
            if (now.Day != user.LastTime.Day)
            {
                user.LastTime = now;
                user.Jrrp = (float)_random.NextDouble();
            }

            await _jrrpDb.SaveChangesAsync();
            return user;
        }

        public void Dispose()
        {
            _jrrpDb.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}