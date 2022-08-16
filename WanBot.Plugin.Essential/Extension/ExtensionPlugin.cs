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

namespace WanBot.Plugin.Essential.Extension
{
    public class ExtensionPlugin : WanBotPlugin, IDisposable
    {
        internal static ExtensionPlugin Instance = null!;

        public override string PluginName => "Extension";

        public override string PluginDescription => "基础扩展插件，提供一些封装的复杂功能";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        /// <summary>
        /// 任务计划器
        /// </summary>
        public Scheduler scheduler = null!;

        internal ConcurrentDictionary<GroupSender, Action<GroupMessage>> waitGroupMessageHandler = new();
        internal ConcurrentDictionary<StrangerSender, Action<TempMessage>> waitTempMessageHandler = new();
        internal ConcurrentDictionary<FriendSender, Action<FriendMessage>> waitFriendMessageHandler = new();

        public override void PreInit()
        {
            scheduler = new(Logger);
            Instance = this;
            base.PreInit();
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

        public void Dispose()
        {
            Instance = null!;
        }
    }
}
