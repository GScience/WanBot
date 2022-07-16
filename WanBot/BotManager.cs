using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;

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

        /// <summary>
        /// 添加账户
        /// </summary>
        /// <param name="miraiConfig"></param>
        /// <exception cref="Exception"></exception>
        internal void AddAccount(MiraiConfig miraiConfig)
        {
            var botLogger = new Logger($"QQ({miraiConfig.QQ})");
            var bot = new MiraiBot(botLogger);
            
            try
            {
                botLogger.Info($"Try add bot {miraiConfig.QQ}");
                bot.ConnectAsync(miraiConfig).Wait();
                var aboutResult = bot.AboutAsync().Result;
                var version = Version.Parse(aboutResult.Data!.Version);
                if (version.Major < 2)
                {
                    throw new Exception("Mirai http 1.x.x is no longer support");
                }

                var botProfile = bot.BotProfileAsync().Result;
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

        public MiraiEventHandler Subscript<T>(int priority, Func<MiraiEventArgs<T>, Task> func) 
            where T : BaseEvent
        {
            var handler = new MiraiEventHandler<T>(priority, func);
            foreach (var bot in _bots)
                bot.Subscripe(handler);

            return handler;
        }

        public void Dispose()
        {
            foreach (var bot in _bots)
                bot.Dispose();
        }
    }
}
