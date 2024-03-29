﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Event
{
    /// <summary>
    /// 为bot增加事件支持的插件
    /// </summary>
    public class SimpleEventsPlugin : WanBotPlugin
    {
        /// <summary>
        /// 正则表
        /// </summary>
        internal List<RegexAttribute> RegexTable { get; } = new();

        public override string PluginName => "SimpleEvents";

        public override string PluginAuthor => "WanNeng";

        public override string PluginDescription => "提供对命令、关键词、At事件的支持";

        public override Version PluginVersion => Version.Parse("1.0.0");


        private CommandEventConfig _config = null!;

        public string CommandPrefix => _config.CommandPrefix;

        public override void PreInit()
        {
            base.PreInit();
            _config = GetConfig<CommandEventConfig>();
        }

        public override void Start()
        {
            base.Start();
            RegexTable.Sort((a, b) => b.Priority - a.Priority);
        }

        private Regex? FindMatchedRegex(MessageChain chain)
        {
            foreach (var regex in RegexTable)
                if (chain.Match(regex.Regex))
                    return regex.Regex;
            return null;
        }

        [MiraiEvent<TempMessage>(Priority.Highest)]
        public async Task OnTempMessage(MiraiBot bot, TempMessage e)
        {
            // 命令
            if (e.MessageChain.StartsWith(_config.CommandPrefix))
            {
                var sender = new StrangerSender(
                    bot,
                    "[Temp]" + e.Sender.GetFormatedName(),
                    e.Sender.MemberName,
                    e.Sender.Id,
                    e.Sender.Group.Id);

                await OnCommandMessage(bot, sender, e.MessageChain);
                e.Blocked = true;
                return;
            }
            // 正则
            var matchRegex = FindMatchedRegex(e.MessageChain);
            if (matchRegex != null)
            {
                var eventArgs = new RegexEventArgs(
                        new StrangerSender(
                            bot,
                            "[Temp]" + e.Sender.GetFormatedName(),
                            e.Sender.MemberName,
                            e.Sender.Id,
                            e.Sender.Group.Id),
                    e.MessageChain,
                    matchRegex);
                await bot.PublishAsync(
                    eventArgs.GetEventName(),
                    eventArgs
                    );
                e.Blocked = true;
                return;
            }
        }

        [MiraiEvent<FriendMessage>(Priority.Highest)]
        public async Task OnFriendMessage(MiraiBot bot, FriendMessage e)
        { 
            // 命令
            if (e.MessageChain.StartsWith(_config.CommandPrefix))
            {
                var sender = new FriendSender(
                    bot,
                    "[Friend]" + e.Sender.GetFormatedName(),
                    e.Sender.Nickname,
                    e.Sender.Id);

                await OnCommandMessage(bot, sender, e.MessageChain);
                e.Blocked = true;
                return;
            }

            // 正则
            var matchRegex = FindMatchedRegex(e.MessageChain);
            if (matchRegex != null)
            {
                var eventArgs = new RegexEventArgs(
                        new FriendSender(
                            bot,
                            "[Friend]" + e.Sender.GetFormatedName(),
                            e.Sender.Nickname,
                            e.Sender.Id),
                    e.MessageChain,
                    matchRegex);
                await bot.PublishAsync(
                    eventArgs.GetEventName(),
                    eventArgs
                    );
                e.Blocked = true;
                return;
            }
        }

        [MiraiEvent<GroupMessage>(Priority.Highest)]
        public async Task OnGroupMessage(MiraiBot bot, GroupMessage e)
        {
            // 命令
            if (e.MessageChain.StartsWith(_config.CommandPrefix))
            {
                var sender = new GroupSender(
                    bot,
                    "[Group]" + e.Sender.GetFormatedName(),
                    e.Sender.MemberName,
                    e.Sender.Group.Id,
                    e.Sender.Id,
                    e.Sender.Permission);

                await OnCommandMessage(bot, sender, e.MessageChain);
                e.Blocked = true;
                return;
            }

            // At
            var begin = e.MessageChain.FirstNotOf<Source>();

            if (begin is At at && at.Target == bot.Id)
            {
                await bot.PublishAsync(
                    typeof(AtEventArgs),
                    new AtEventArgs(
                        new GroupSender(
                            bot,
                            "[Group]" + e.Sender.GetFormatedName(),
                            e.Sender.MemberName,
                            e.Sender.Group.Id,
                            e.Sender.Id,
                            e.Sender.Permission),
                    e.MessageChain));
                e.Blocked = true;
                return;
            }

            // 正则
            var matchRegex = FindMatchedRegex(e.MessageChain);
            if (matchRegex != null)
            {
                var eventArgs = new RegexEventArgs(
                        new GroupSender(
                            bot,
                            "[Group]" + e.Sender.GetFormatedName(),
                            e.Sender.MemberName,
                            e.Sender.Group.Id,
                            e.Sender.Id,
                            e.Sender.Permission),
                    e.MessageChain,
                    matchRegex);
                await bot.PublishAsync(
                    eventArgs.GetEventName(),
                    eventArgs);
                e.Blocked = true;
                return;
            }
        }

        [MiraiEvent<NudgeEvent>]
        public async Task OnNudge(MiraiBot bot, NudgeEvent e)
        {
            if (e.Target != bot.Id)
                return;

            ISender sender;

            switch (e.Subject.Kind)
            {
                case "Friend":
                    sender = new FriendSender(bot, "", "", e.Subject.Id);
                    break;

                case "Group":
                    sender = new GroupSender(bot, "", "", e.Subject.Id, e.FromId, "MEMBER");
                    break;

                case "Stranger":
                    sender = new StrangerSender(bot, "", "", e.Subject.Id, e.FromId);
                    break;

                default:
                    return;
            }

            var eventArgs = new NudgeEventArgs(sender);
            await bot.PublishAsync(
                typeof(NudgeEventArgs),
                eventArgs
                );
        }

        public async Task OnCommandMessage(MiraiBot bot, ISender sender, MessageChain messageChain)
        {
            var divider = new MessageChainDivider(messageChain);
            var firstChain = divider.ReadNext();
            if (firstChain is not string firstStr)
                throw new Exception("Failed to get first string of MessageChain.");

            var cmd = firstStr[_config.CommandPrefix.Length..];
            if (cmd.Length == 0)
                return;

            Logger.Info($"{sender.InternalName} do command #{{cmd}}", cmd);

            var eventArgs = new CommandEventArgs(sender, divider, cmd);
            try
            {
                await bot.PublishAsync(eventArgs.GetEventName(), eventArgs);

                if (!eventArgs.Blocked)
                    Logger.Warn($"Command #{{cmd}} not found, or not blocked by the handle", cmd);
            }
            catch (Exception e)
            {
                Logger.Warn("Error while deal with command #{cmd}", cmd);

                var errMsg = e.Message;
                if (errMsg.Length > 20)
                    errMsg = errMsg[0..17] + "...";
                await sender.ReplyAsync($"好奇怪，{errMsg}", messageChain.MessageId);
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
