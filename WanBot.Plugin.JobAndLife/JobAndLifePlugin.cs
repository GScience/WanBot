using System;
using System.Numerics;
using System.Reflection;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Plugin.Essential.EssAttribute;
using WanBot.Plugin.Essential.Extension;
using WanBot.Plugin.Essential.Permission;
using WanBot.Plugin.Jrrp;

namespace WanBot.Plugin.JobAndLife
{
    public class JobAndLifePlugin : WanBotPlugin
    {
        internal static DateTime StartTime = new DateTime(2021, 10, 1);
        internal static double BaseSalary = 200;

        private Random _random = new();

        public override string PluginName => "JobAndLife";

        public override string PluginDescription => "生活与工作插件，包含打工、休息等基础功能";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private EssAttrUserFactory _attrUsr = null!;
        private JrrpAddition? _jrrpAddition;

        /// <summary>
        /// 提现量表
        /// </summary>
        private int _withdrawalCount;

        private string[] _withdrawalLevel = new[]
        {
            "提现",
            "金元宝",
            "铂金",
            "钻石",
            "矿场",
            "银行",
            "宇宙飞船",
            "调试控制台",
            "DotNet",
            "完犊子"
        };

        public override void Start()
        {
            this.GetBotHelp()
                .Category("工作和生活")
                .Command("#打工", "尝试从资本家手里抢钱")
                .Command("#加班", "尝试继续从资本家手里抢钱")
                .Command("#旷工", "老子今天不上班");

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

        [Command("旷工")]
        public async Task OnNotWorkCommand(MiraiBot bot, CommandEventArgs args)
        {
            /*
            if (!args.Sender.HasPermission(this, "User"))
            {
                await args.Sender.ReplyAsync("完犊子，你没有 旷工 的权限");
                return;
            }
            args.Blocked = true;

            await using var usr = _attrUsr.FromSender(args.Sender);

            if (usr.Money < 0)
            {
                await args.Sender.ReplyAsync("你没钱了，哪都去不了");
            }
            else if (usr.Energy >= usr.EnergyMax)
            {
                usr.Money -= 2000;
                await args.Sender.ReplyAsync("你一点都不累还旷工，老板扣了你 2000 元");
            }
            else if (usr.Energy >= 0)
            {
                usr.Money -= 1500;
                usr.Energy = usr.EnergyMax;
                await args.Sender.ReplyAsync("你没有那么累还旷工，老板扣了你 1500 元");
            }
            else if (usr.Energy < 0)
            {
                usr.Money -= 500;
                usr.Energy += 50;
                await args.Sender.ReplyAsync("看在你那么累的份上，老板好心只扣了你 500 元");
            }
            */
            await args.Sender.ReplyAsync("不允许旷工");
        }

        [Command("加班")]
        public async Task OnOverworkCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasPermission(this, "User"))
            {
                await args.Sender.ReplyAsync("完犊子，你没有 加班 的权限");
                return;
            }
            args.Blocked = true;
            if (!await DoWork(args.Sender, true))
                await args.Sender.ReplyAsync("加不动班了");
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
            if (!await DoWork(args.Sender, false))
                await args.Sender.ReplyAsync("你感觉很虚弱，还想干活请加班");
        }

        [Command("提现")]
        public async Task OnWithdrawalCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasPermission(this, "User"))
            {
                await args.Sender.ReplyAsync("完犊子，你没有 提现 的权限");
                return;
            }
            args.Blocked = true;
            if (_withdrawalCount == 0)
            {
                await args.Sender.ReplyAsync($"已超时，重置助力次数，请输入5次{_withdrawalLevel[0]}，已输入1次");
                _withdrawalCount = 1;
            }
            else
            {
                var currentLevel = _withdrawalCount / 5;
                ++_withdrawalCount;
                var newLevel = _withdrawalCount / 5;
                if (newLevel >= _withdrawalLevel.Length)
                    await args.Sender.ReplyAsync(
                        $"太遗憾了，没找到{_withdrawalLevel[_withdrawalLevel.Length - 1]}，继续努力哦~");
                else if (currentLevel == newLevel)
                {
                    BigInteger money = 0;
                    using (var usr = _attrUsr.FromSender(args.Sender)) money = usr.Money;
                    await args.Sender.ReplyAsync(
                        $"当前已收集{_withdrawalCount % 5}/5个\"{_withdrawalLevel[currentLevel]}\"加油，马上就能拿{money}！");
                }
                else
                    await args.Sender.ReplyAsync(
                        $"太难收集了？收集概率更大的\"{_withdrawalLevel[newLevel]}\"吧，满5个可换1个\"{_withdrawalLevel[currentLevel]}\"");
            }
        }

        /// <summary>
        /// 计算工资
        /// </summary>
        /// <returns></returns>
        private double GetSalary()
        {
            // 根据天数进行波动，按照整小时进行离散运算
            var d = Math.Floor((DateTime.Now - StartTime).TotalHours) / 24;
            // 通货膨胀率（日）
            var k2 = 0.01;
            // 虚犊币膨胀后的价格
            var r1 = BaseSalary + BaseSalary * d * k2;

            return r1;
        }

        public async Task<bool> DoWork(ISender sender, bool overwork)
        {
            using var usr = _attrUsr.FromSender(sender);

            // 能量小于0禁止打工
            if (usr.Energy < 0)
                return false;

            // 能量小于最大值并且不允许加班禁止打工
            if (usr.Energy < usr.EnergyMax && !overwork)
                return false;

            usr.Energy -= 50;

            var jrrpScale = _jrrpAddition == null ? 1 : await _jrrpAddition.GetJrrpSalaryScale(sender.Id);
            var salary = GetSalary() * jrrpScale;
            if (usr.Energy < 0)
            {
                var costMoney = (BigInteger)(salary * 0.8);
                usr.Money -= costMoney;
                await sender.ReplyAsync($"完犊子了，累到脑溢血还不算工伤，亏了 {costMoney} 元");
            }
            else if (usr.Energy < 30)
            {
                var rand = (jrrpScale > 1) ? 
                    _random.Next(0, 2) : 
                    _random.Next(0, 5);

                switch (rand)
                {
                    case 0:
                        await sender.ReplyAsync("老板没看到你加班，不给钱");
                        break;
                    case 1:
                        var luckMoney1 = (BigInteger)(salary * _random.Next(1, 500) / 10.0);
                        usr.Money += luckMoney1;
                        await sender.ReplyAsync($"你加班干了个私活，赚了 {luckMoney1} 元");
                        break;
                    case 2:
                        var unluckMoney1 = (BigInteger)(salary * _random.Next(1, 90) / 100.0);
                        usr.Money -= unluckMoney1;
                        await sender.ReplyAsync($"没赶上末班车，打车花了 {unluckMoney1} 元");
                        break;
                    case 3:
                        var unluckMoney2 = (BigInteger)(salary * _random.Next(1, 10) / 100.0);
                        usr.Money -= unluckMoney2;
                        await sender.ReplyAsync($"钱包被人拿走了，丢了 {unluckMoney2} 元");
                        break;
                    case 4:
                        var luckMoney2 = (BigInteger)(salary * _random.Next(1, 10) / 100.0);
                        usr.Money += luckMoney2;
                        await sender.ReplyAsync($"捡走了某人的钱包，赚了 {luckMoney2} 元");
                        break;
                }
            }
            else
            {
                usr.Money += (BigInteger)salary;
                await sender.ReplyAsync($"今天也是辛苦的一天，赚了 {(BigInteger)salary} 元");
            }

            return true;
        }
    }
}