using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using WanBot.Graphic;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.Essential.Extension
{
    public class ExtensionPlugin : WanBotPlugin, IDisposable
    {
        internal static ExtensionPlugin Instance = null!;

        public override string PluginName => "Extension";

        public override string PluginDescription => "基础扩展插件，提供一些封装的复杂功能";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        public BotHelp botHelp = new();

        /// <summary>
        /// 任务计划器
        /// </summary>
        public Scheduler scheduler = null!;

        private UIRenderer? _renderer;

        internal ConcurrentDictionary<GroupSender, Action<GroupMessage>> waitGroupMessageHandler = new();
        internal ConcurrentDictionary<StrangerSender, Action<TempMessage>> waitTempMessageHandler = new();
        internal ConcurrentDictionary<FriendSender, Action<FriendMessage>> waitFriendMessageHandler = new();

        public override void PreInit()
        {
            scheduler = new(Logger);
            Instance = this;

            botHelp.Category("完犊子Bot的食用方式");
            botHelp.Command("#help", "查看Bot的帮助");
            base.PreInit();
        }

        public override void Start()
        {
            _renderer = this.GetUIRenderer();
            base.Start();
        }

        [MiraiEvent<GroupMessage>(Priority.Highest + 10)]
        public Task OnMiraiGroupMessage(MiraiBot bot, GroupMessage args)
        {
            var sender = new GroupSender(bot, "", "", args.Sender.Group.Id, args.Sender.Id);
            if (waitGroupMessageHandler.TryRemove(sender, out var action))
                action(args);
            return Task.CompletedTask;
        }

        [MiraiEvent<TempMessage>(Priority.Highest + 10)]
        public Task OnMiraiTempMessage(MiraiBot bot, TempMessage args)
        {
            var sender = new StrangerSender(bot, "", "", args.Sender.Group.Id, args.Sender.Id);
            if (waitTempMessageHandler.TryRemove(sender, out var action))
                action(args);
            return Task.CompletedTask;
        }

        [MiraiEvent<FriendMessage>(Priority.Highest + 10)]
        public Task OnMiraiFriendMessage(MiraiBot bot, FriendMessage args)
        {
            var sender = new FriendSender(bot, "", "", args.Sender.Id);
            if (waitFriendMessageHandler.TryRemove(sender, out var action))
                action(args);
            return Task.CompletedTask;
        }

        [Command("help", "帮助")]
        public async Task OnHelpCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "help"))
                return;

            args.Blocked = true;
            var msgBuilder = new MessageBuilder();
            using var miraiImage = new MiraiImage(bot, botHelp.GetHelpImage(_renderer!), false);
            msgBuilder.Image(miraiImage);
            await args.Sender.ReplyAsync(msgBuilder);
        }

        public void Dispose()
        {
            Instance = null!;
            scheduler?.Dispose();
        }

        public override void Stop()
        {
            botHelp.Dispose();
        }
    }
}
