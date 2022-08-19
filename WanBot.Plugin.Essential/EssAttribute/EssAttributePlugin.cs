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

        internal EssAttributeDatabaseContext? _essAttrDb;

        public EssAttrUserFactory? essAttrUsrFactory;

        public override void PreInit()
        {
            _essAttrDb = new EssAttributeDatabaseContext(Path.Combine(GetConfigPath(), "essAttr.db"));
            _essAttrDb.Database.Migrate();

            essAttrUsrFactory = new(_essAttrDb);
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

            await using var essUsr = essAttrUsrFactory!.FromSender(commandEvent.Sender);
            await commandEvent.Sender.ReplyAsync($"你有 {essUsr.Money} 元");
        }

        [Command("体力")]
        public async Task OnEnergyCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!commandEvent.Sender.HasCommandPermission(this, "energy"))
                return;

            commandEvent.Blocked = true;

            await using var essUsr = essAttrUsrFactory!.FromSender(commandEvent.Sender);
            await commandEvent.Sender.ReplyAsync($"你有 {(int)((float)essUsr.Energy / essUsr.EnergyMax * 100)}% 的体力");
        }

        public void Dispose()
        {
            _essAttrDb?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
