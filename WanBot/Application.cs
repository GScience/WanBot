using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;
using WanBot.Util;

namespace WanBot
{
    public class Application : IApplication, IDisposable
    {
        /// <summary>
        /// Bot列表
        /// </summary>
        internal List<MiraiBot> bots = new();

        /// <summary>
        /// 日志
        /// </summary>
        private Logger _logger = new Logger("WanBot");

        /// <summary>
        /// 配置路径
        /// </summary>
        internal string ConfigPath { get; set; } = Path.Combine(Environment.CurrentDirectory, "Config");

        private Semaphore _consoleCloseSemaphore = new(0, 1);
        private bool _isClosing = false;

        /// <summary>
        /// 从相对路径读取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relvPath"></param>
        internal T ReadConfigFromFile<T>(string relvPath) where T : new()
        {
            return ConfigHelper.ReadConfigFromFile<T>(Path.Combine(ConfigPath, relvPath));
        }

        internal void CheckDir()
        {
            if (!Directory.Exists(ConfigPath))
                Directory.CreateDirectory(ConfigPath);
        }

        /// <summary>
        /// 启动应用
        /// </summary>
        internal void Start()
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                _logger.Info("Get user cancel requirement");
                if (!_isClosing)
                    OnConsoleExit();
            };

            CheckDir();

            // 创建Bot
            _logger.Info("Loading WanBot Config and connecting to mirai");

            var config = ReadConfigFromFile<WanBotConfig>("WanBot.conf");
            foreach (var miraiConfig in config.MiraiConfigs)
            {
                var botLogger = new Logger($"QQ({miraiConfig.QQ})");
                var bot = new MiraiBot(botLogger);
                bots.Add(bot);
                try
                {
                    _logger.Info($"Try add bot {miraiConfig.QQ}");
                    bot.ConnectAsync(miraiConfig).Wait();
                    var version = Version.Parse(bot.AboutAsync().Result.Version);
                    if (version.Major < 2)
                    {
                        _logger.Error("Mirai http {version} is no longer support", "1.x.x");
                        continue;
                    }

                    var botProfile = bot.BotProfileAsync().Result;
                    botLogger.SetCategory($"{botProfile.Nickname}({miraiConfig.QQ})");

                    botLogger.Info("Mirai http version: {version}", version);
                    botLogger.Info("Bot {Nickname} connected", botProfile.Nickname);
                }
                catch (AggregateException e)
                {
                    _logger.Info("Failed to add bot {miraiConfig} because {e}", miraiConfig.QQ, e);
                    continue;
                }
            }

            _logger.Info("All done");

            // 等待程序退出
            _consoleCloseSemaphore.WaitOne();
        }

        internal void OnConsoleExit()
        {
            _isClosing = true;
            _logger.Info("Sending close signal");
            _consoleCloseSemaphore.Release(1);
        }

        public void Dispose()
        {
            _logger.Info("Dispose resources");

            _consoleCloseSemaphore.Dispose();
            foreach (var bot in bots)
                bot.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
