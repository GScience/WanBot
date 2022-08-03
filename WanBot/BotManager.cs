using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;

namespace WanBot
{
    /// <summary>
    /// 机器人管理器
    /// </summary>
    internal class BotManager : IBotManager, IDisposable
    {
        /// <summary>
        /// Bot列表
        /// </summary>
        private List<MiraiBot> _bots = new();
        public void Reload(bool disconnect)
        {
            foreach (var bot in _bots)
            {
                if (disconnect)
                    bot.Dispose();
                else
                    bot.ClearEventHandle();
            }

            if (disconnect)
                _bots.Clear();
        }

        /// <summary>
        /// 添加账户
        /// </summary>
        /// <param name="miraiConfig"></param>
        /// <exception cref="Exception"></exception>
        internal async Task AddAccountAsync(MiraiConfig miraiConfig)
        {
            var botLogger = new Logger($"QQ({miraiConfig.QQ})");
            var bot = new MiraiBot(botLogger);
            
            try
            {
                botLogger.Info($"Try add bot {miraiConfig.QQ}");
                await bot.ConnectAsync(miraiConfig);
                botLogger.Info($"Checking mirai http version");

                var aboutResult = await bot.AboutAsync();
                var version = Version.Parse(aboutResult.Data!.Version);
                if (version.Major < 2)
                {
                    throw new Exception("Mirai http 1.x.x is no longer support");
                }

                var botProfile = await bot.BotProfileAsync();
                botLogger.SetCategory($"{botProfile.Nickname}({miraiConfig.QQ})");

                botLogger.Info("Mirai http version: {version}", version);
                botLogger.Info("Bot {Nickname} connected", botProfile.Nickname);

                _bots.Add(bot);
            }
            finally
            {
                if (!_bots.Contains(bot))
                    bot.Dispose();
            }
        }

        public WanBotEventHandler Subscript<T>(int priority, Func<MiraiBot, T, Task> func)
            where T : BlockableEventArgs
        {
            var handler = new MiraiEventHandler<T>(priority, func);
            foreach (var bot in _bots)
                bot.Subscripe(handler);

            return handler;
        }

        public WanBotEventHandler Subscript(Type type, int priority, Func<MiraiBot, BlockableEventArgs, Task> func)
        {
            var handler = new WanBotEventHandler(priority, func);
            foreach (var bot in _bots)
                bot.Subscripe(type, handler);

            return handler;
        }

        public WanBotEventHandler Subscript(string eventName, int priority, Func<MiraiBot, BlockableEventArgs, Task> func)
        {
            var handler = new WanBotEventHandler(priority, func);
            foreach (var bot in _bots)
                bot.Subscripe(eventName, handler);

            return handler;
        }

        public void Dispose()
        {
            foreach (var bot in _bots)
                bot.Dispose();
        }
    }
}
