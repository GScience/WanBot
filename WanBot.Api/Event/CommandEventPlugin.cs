using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Event
{
    /// <summary>
    /// 命令支持插件
    /// </summary>
    public class CommandEventPlugin : WanBotPlugin
    {
        public override string PluginName => "Command";
        public override string PluginAuthor => "WanNeng";
        public override Version PluginVersion => Version.Parse("1.0.0");

        private CommandEventConfig _config = null!;

        public override void PreInit()
        {
            base.PreInit();
            _config = GetConfig<CommandEventConfig>();
        }

        [MiraiEvent<TempMessage>(Priority.Highest)]
        public async Task OnTempMessage(MiraiBot bot, TempMessage e)
        {
            if (!e.MessageChain.StartsWith(_config.CommandPrefix))
                return;

            var sender = new StrangerSender(
                bot,
                $"{e.Sender.MemberName}({e.Sender.Id})",
                e.Sender.Id,
                e.Sender.Group.Id);

            await OnMessage(bot, sender, e.MessageChain);
        }

        [MiraiEvent<FriendMessage>(Priority.Highest)]
        public async Task OnFriendMessage(MiraiBot bot, FriendMessage e)
        {
            if (!e.MessageChain.StartsWith(_config.CommandPrefix))
                return;

            var sender = new FriendSender(
                bot,
                $"{e.Sender.Nickname}({e.Sender.Id})",
                e.Sender.Id);

            await OnMessage(bot, sender, e.MessageChain);
        }
        
        [MiraiEvent<GroupMessage>(Priority.Highest)]
        public async Task OnGroupMessage(MiraiBot bot, GroupMessage e)
        {
            if (!e.MessageChain.StartsWith(_config.CommandPrefix))
                return;

            var sender = new GroupSender(
                bot,
                $"[{e.Sender.Group.Name}({e.Sender.Group.Id})] {e.Sender.MemberName}({e.Sender.Id})",
                e.Sender.Group.Id);

            await OnMessage(bot, sender, e.MessageChain);
        }


        public async Task OnMessage(MiraiBot bot, ISender sender, MessageChain messageChain)
        {
            var divider = new MessageChainDivider(messageChain);
            var firstChain = divider.ReadNext();
            if (firstChain is not string firstStr)
                throw new Exception("Failed to get first string of MessageChain.");

            var cmd = firstStr[_config.CommandPrefix.Length..];
            if (cmd.Length == 0)
                return;

            Logger.Info($"{sender.Name} do command #{{cmd}}", cmd);

            var eventArgs = new CommandEventArgs(sender, divider, cmd);
            try
            {
                await bot.PublishAsync(eventArgs.GetEventName(), eventArgs);
            }
            catch (Exception)
            {
                Logger.Warn("Error while deal with command #{cmd}", cmd);
                throw;
            }
        }
    }

    /// <summary>
    /// 命令事件配置
    /// </summary>
    internal class CommandEventConfig
    {
        /// <summary>
        /// 命令触发词
        /// </summary>
        public string CommandPrefix { get; set; } = "#";
    }
}
