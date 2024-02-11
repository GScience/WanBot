using System;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;
using WanBot.Graphic.Template;
using WanBot.Plugin.AI.AIAdapter;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.AI
{
    public class AIPlugin : WanBotPlugin
    {
        public override string PluginName => "AIPlugin";

        public override string PluginDescription => "智慧完犊子";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private AIConfig? _config;

        private IAIAdapter? _aiAdapter;

        private ChatHistoryDictionary _chatHistory = new();

        public override void PreInit()
        {
            base.PreInit();
            _config = GetConfig<AIConfig>();
            if (!string.IsNullOrWhiteSpace(_config.TongYiApiKey))
            {
                _aiAdapter = new TongYi(_config.TongYiApiKey, _config.TongYiModel);
            }
            else
            {
                Logger.Warn("No AI config has found");
            }
        }

        [Command("你怎么看")]
        public async Task OnWhatYouThink(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!commandEvent.Sender.HasPermission(this, "ai"))
                return;
            if (commandEvent.Sender is not GroupSender groupSender)
                return;
            commandEvent.Blocked = true;
            if (_aiAdapter == null || _config == null)
            {
                await commandEvent.Sender.ReplyAsync("有八嘎没认真配置Api key");
                return;
            }
            try
            {
                var groupChatHistory = _chatHistory.GetChatHistory(groupSender.GroupId);
                var aiResponse = await _aiAdapter.ProcessAsync(
                    _config.SystemPrompt,
                    groupChatHistory);
                await commandEvent.Sender.ReplyAsync(aiResponse);
                _chatHistory.LogChat(groupSender.GroupId, aiResponse, true);
            }
            catch (Exception ex)
            {
                await commandEvent.Sender.ReplyAsync("呃啊，坏掉了");
                Logger.Error("{0}", ex);
            }
        }

        [MiraiEvent<GroupMessage>(Priority.Lowest)]
        public async Task OnGroupMessage(MiraiBot bot, GroupMessage e)
        {
            var chatStr = await FormatMessageChainAsync(bot, e.Sender.Group.Id, e.Sender.MemberName, e.MessageChain);
            _chatHistory.LogChat(e.Sender.Group.Id, chatStr, false);
        }

        [Regex("完犊子")]
        public async Task OnMentionWanBot(MiraiBot bot, RegexEventArgs atEvent)
        {
            if (!atEvent.Sender.HasPermission(this, "ai"))
                return;
            if (atEvent.Sender is not GroupSender groupSender)
                return;
            atEvent.Blocked = true;
            if (_aiAdapter == null || _config == null)
            {
                await atEvent.Sender.ReplyAsync("有八嘎没认真配置Api key");
                return;
            }
            try
            {
                var chatStr = await FormatMessageChainAsync(bot, groupSender.GroupId, atEvent.Sender.DisplayName, atEvent.Chain);
                _chatHistory.LogChat(groupSender.GroupId, chatStr, false);
                var groupChatHistory = _chatHistory.GetChatHistory(groupSender.GroupId);
                var aiResponse = await _aiAdapter.ProcessAsync(
                    _config.SystemPrompt,
                    groupChatHistory);
                await atEvent.Sender.ReplyAsync(aiResponse);
                _chatHistory.LogChat(groupSender.GroupId, aiResponse, true);
            }
            catch (Exception ex)
            {
                await atEvent.Sender.ReplyAsync("呃啊，坏掉了");
                Logger.Error("{0}", ex);
            }
        }


        public static async Task<string> FormatMessageChainAsync(MiraiBot bot, long groupId, string senderName, MessageChain messageChain)
        {
            var chatStr = $"<{senderName}>";
            foreach (var chain in messageChain)
                if (chain is Plain plainChain)
                    chatStr += plainChain.Text;
                else if (chain is At atChain)
                {
                    var profile = await bot.MemberProfileAsync(groupId, atChain.Target);
                    chatStr += $"@{profile.Nickname}";
                }
                else if (chain is Source sourceChain)
                {
                    chatStr += $"<{DateTimeOffset.FromUnixTimeSeconds(sourceChain.Time).LocalDateTime}>";
                }
                else
                    chatStr += $"[{chain.GetType().Name}]";
            return chatStr;
        }
    }
}
