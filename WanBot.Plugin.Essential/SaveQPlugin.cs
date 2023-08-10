using System;
using System.Buffers.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.Words;
using WanBot.Api;
using WanBot.Api.Hook;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;
using WanBot.Api.Mirai.Payload;

namespace WanBot.Plugin.Essential
{
    /// <summary>
    /// 账号安全插件
    /// </summary>
    internal class SaveQPlugin : WanBotPlugin
    {
        public override string PluginName => "SaveQ";

        public override string PluginDescription => "保护账号安全";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private ConcurrentDictionary<long, SemaphoreSlim> _groupSemaphore = new();
        private SemaphoreSlim _semaphore = null!;
        private SaveQConfig _config = null!;
        private long _waitCount = 0;
        private WordsSearch _wordSearch = null!;

        public override void PreInit()
        {
            _semaphore = new(1, 1);
            _wordSearch = new();
            _config = GetConfig<SaveQConfig>();
            _wordSearch.SetKeywords(LoadBannedWords());
            base.PreInit();
        }

        public override void Start()
        {
            HookTable.Instance.ApiHook = ApiHookAsync;
        }

        private ICollection<string> LoadBannedWords()
        {
            Logger.Info("正在加载违禁词词库");
            var bannedWordPath = Path.Combine(GetConfigPath(), "BannedWords.txt");
            if (!System.IO.File.Exists(bannedWordPath))
            {
                System.IO.File.WriteAllText(bannedWordPath, "6K+35L2/55SoYmFzZTY05Yqg5a+G");
                Logger.Warn($"未找到违禁词词库，在{bannedWordPath}创建，请使用base64进行加密");
            }
            var bannedWordsBase64 = System.IO.File.ReadAllLines(bannedWordPath);
            var result = bannedWordsBase64.Select(w => Encoding.UTF8.GetString(Convert.FromBase64String(w))).ToArray();
            Logger.Info($"总共加载违禁词 {result.Length} 个");
            return result;
        }

        public override void Stop()
        {
            base.Stop();
            _semaphore?.Dispose();
            foreach (var item in _groupSemaphore.Values)
                item.Dispose();
            _groupSemaphore.Clear();
        }

        private async Task<object?> ApiHookAsync(MiraiBot bot, object args)
        {
            if (args is SendGroupMessageRequest groupMessageRequest)
                return await OnBotSendGroupMessage(groupMessageRequest);
            if (args is SendFriendMessageRequest friendMessageRequest)
                return await OnBotSendFriendMessage(friendMessageRequest);
            if (args is SendTempMessageRequest tempMessageRequest)
                return await OnBotSendTempMessage(tempMessageRequest);
            return args;
        }

        private async Task<SendGroupMessageRequest?> OnBotSendGroupMessage(SendGroupMessageRequest request)
        {
            if (request.MessageChain == null)
                return null;
            request.MessageChain = await OnSendMessage(request.MessageChain, request.Target);
            return request;
        }

        private async Task<SendFriendMessageRequest?> OnBotSendFriendMessage(SendFriendMessageRequest request)
        {
            if (request.MessageChain == null)
                return null;
            request.MessageChain = await OnSendMessage(request.MessageChain, request.Target);
            return request;
        }

        private async Task<SendTempMessageRequest?> OnBotSendTempMessage(SendTempMessageRequest request)
        {
            if (request.MessageChain == null)
                return null;
            request.MessageChain = await OnSendMessage(request.MessageChain, request.QQ);
            return request;
        }

        private SemaphoreSlim SlimFactory(long targetId)
        {
            return new SemaphoreSlim(1, 1);
        }

        private async Task<MessageChain> OnSendMessage(MessageChain msg, long targetId)
        {
            var groupSemaphore = _groupSemaphore.GetOrAdd(targetId, SlimFactory);

            if (Interlocked.Read(ref _waitCount) > _config!.MaxWaitCount)
                throw new Exception("超出消息限制");

            Interlocked.Increment(ref _waitCount);
            try
            {
                await groupSemaphore!.WaitAsync();
                await _semaphore!.WaitAsync();
            }
            finally
            {
                Interlocked.Decrement(ref _waitCount);
            }

            // 不需要等待
            _ = Task.Delay(_config!.MessageSendIntervalGlobal).ContinueWith(t => _semaphore.Release());
            _ = Task.Delay(_config!.MessageSendIntervalPersion).ContinueWith(t => groupSemaphore.Release());

            // 进行敏感词审查
            foreach (var chain in msg)
                if (chain is Plain plain)
                    plain.Text = GetSafeString(plain.Text);

            // 返回消息
            return msg;
        }

        private string GetSafeString(string msg)
        {
            try
            {
                msg = _wordSearch.Replace(msg, 'x');
                return msg;
            }
            catch (Exception ex)
            {
                throw new Exception("安全检查失败", ex);
            }
        }
    }

    public class SaveQConfig
    {
        /// <summary>
        /// 全局消息发送间隔
        /// </summary>
        public int MessageSendIntervalGlobal { get; set; } = 5000;

        /// <summary>
        /// 群消息发送间隔
        /// </summary>
        public int MessageSendIntervalPersion { get; set; } = 10000;

        /// <summary>
        /// 最大的等待数量
        /// </summary>
        public int MaxWaitCount { get; set; } = 25;
    }
}
