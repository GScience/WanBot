using System;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Plugin.Essential.EssAttribute;
using WanBot.Plugin.Essential.Extension;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.JobAndLife
{
    public class JobAndLifePlugin : WanBotPlugin
    {
        private Random _random = new();

        public override string PluginName => "JobAndLife";

        public override string PluginDescription => "生活与工作插件，包含打工、休息等基础功能";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private EssAttrUserFactory _attrUsr = null!;

        public override void Start()
        {
            this.GetBotHelp()
                .Category("工作和生活")
                .Command("#打工", "尝试从资本家手里抢钱");

            _attrUsr = this.GetEssAttrUserFactory();
            base.Start();
        }

        [Command("打工")]
        public async Task OnWorkCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasPermission(this, "User"))
            {
                await args.Sender.ReplyAsync("完犊子，你没有 打工 的权限");
                return;
            }
            args.Blocked = true;

            await using var usr = _attrUsr.FromSender(args.Sender);

            if (usr.Energy < 0)
            {
                await args.Sender.ReplyAsync("打不动了");
                return;
            }

            usr.Energy -= 50;

            if (usr.Energy < 0)
            {
                usr.Money -= 1000;
                await args.Sender.ReplyAsync("完犊子了，累到脑淤血还不算工伤，亏了 1000 元");
            }
            else if (usr.Energy < 30)
            {
                switch (_random.Next(0, 5))
                {
                    case 0:
                        await args.Sender.ReplyAsync("老板没看到你加班，不给钱");
                        break;
                    case 1:
                        var luckMoney1 = _random.Next(100, 5000);
                        usr.Money += luckMoney1;
                        await args.Sender.ReplyAsync($"你加班干了个私活，赚了 {luckMoney1} 元");
                        break;
                    case 2:
                        var unluckMoney1 = -_random.Next(50, 250);
                        usr.Money += unluckMoney1;
                        await args.Sender.ReplyAsync($"没赶上末班车，打车花了 {unluckMoney1} 元");
                        break;
                    case 3:
                        var unluckMoney2 = -_random.Next(5, 50);
                        usr.Money += unluckMoney2;
                        await args.Sender.ReplyAsync($"钱包被人拿走了，丢了 {unluckMoney2} 元");
                        break;
                    case 4:
                        var luckMoney2 = _random.Next(5, 50);
                        usr.Money += luckMoney2;
                        await args.Sender.ReplyAsync($"捡走了某人的钱包，赚了 {luckMoney2} 元");
                        break;
                }
            }
            else
            {
                usr.Money += 1000;
                await args.Sender.ReplyAsync("今天也是辛苦的一天，赚了 1000 元");
            }
        }
    }
}