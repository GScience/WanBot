using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Plugin.Essential.Extension;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.Essential.EssAttribute
{
    public class EssAttributePlugin : WanBotPlugin, IDisposable
    {
        public override string PluginName => "EssAttribute";

        public override string PluginDescription => "负责存储用户的基础属性值，包括但不限于金钱等，为通用库";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        public EssAttrUserFactory? essAttrUsrFactory;

        public override void PreInit()
        {
            var dbPath = Path.Combine(GetConfigPath(), "essAttr.db");
            using var essAttrDb = new EssAttributeDatabaseContext(dbPath);
            essAttrDb.Database.Migrate();
            essAttrDb.SaveChanges();

            essAttrUsrFactory = new(dbPath);
            base.PreInit();
        }

        public override void Start()
        {
            this.GetBotHelp()
                .Category("基础属性")
                .Command("#钱", "看看自己有多少钱")
                .Command("#体力", "看看自己还有多少\"价值\"")
                .Info("呜哇我钱呢？");

            base.Start();
        }

        [Command("钱")]
        public async Task OnMoneyCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!commandEvent.Sender.HasCommandPermission(this, "money"))
                return;

            commandEvent.Blocked = true;

            using var essUsr = essAttrUsrFactory!.FromSender(commandEvent.Sender);
            await commandEvent.Sender.ReplyAsync($"你有 {essUsr.Money} 元");
        }

        [Command("状态")]
        public async Task OnStatusCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!commandEvent.Sender.HasCommandPermission(this, "status"))
                return;

            commandEvent.Blocked = true;

            using var essUsr = essAttrUsrFactory!.FromSender(commandEvent.Sender);
            await commandEvent.Sender.ReplyAsync($"你已经休息了 {DateTime.Now - essUsr._user.LastTimeCheckEnergy} 时间，休息满1小时即可恢复体力");
        }

        [Command("体力")]
        public async Task OnEnergyCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!commandEvent.Sender.HasCommandPermission(this, "energy"))
                return;

            commandEvent.Blocked = true;

            using var essUsr = essAttrUsrFactory!.FromSender(commandEvent.Sender);
            await commandEvent.Sender.ReplyAsync($"你有 {(int)((float)essUsr.Energy / essUsr.EnergyMax * 100)}% 的体力");
        }

        public void Dispose()
        {
            essAttrUsrFactory?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
