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
        public static Application Current = null!;

        private BotManager _botManager = new();

        /// <summary>
        /// 账户管理器
        /// </summary>
        public IBotManager BotManager => _botManager;

        /// <summary>
        /// 插件管理器
        /// </summary>
        public PluginManager PluginManager { get; } = new();

        /// <summary>
        /// 日志
        /// </summary>
        private Logger _logger = new Logger("WanBot");

        /// <summary>
        /// 配置路径
        /// </summary>
        internal string ConfigPath { get; set; } = Path.Combine(Environment.CurrentDirectory, "Config");

        /// <summary>
        /// 插件路径
        /// </summary>
        internal string PluginPath { get; set; } = Path.Combine(Environment.CurrentDirectory, "Plugin");

        private Semaphore _consoleCloseSemaphore = new(0, 1);
        private bool _isClosing = false;

        public Application()
        {
            Current = this;
        }

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

            var bindAccountTask = BindAccountAsync();

            // 初始化插件系统
            _logger.Info("Loading plugins");
            PluginManager.LoadAssemblysFromDir(PluginPath);
            PluginManager.FindPlugins();
            PluginManager.InitPlugins();

            // 等待账户绑定
            bindAccountTask.Wait();

            PluginManager.PostInitPlugins();
            _logger.Info("All done");

            // 等待程序退出
            _consoleCloseSemaphore.WaitOne();
        }

        /// <summary>
        /// 绑定账户
        /// </summary>
        /// <returns></returns>
        internal async Task BindAccountAsync()
        {
            // 创建Bot
            _logger.Info("Loading WanBot config and connecting to mirai");

            var config = ReadConfigFromFile<WanBotConfig>("WanBot.conf");
            var tasks = new List<Task>();
            foreach (var miraiConfig in config.MiraiConfigs)
            {
                try
                {
                    tasks.Add(_botManager.AddAccountAsync(miraiConfig));
                }
                catch (Exception e)
                {
                    _logger.Error(e.ToString());
                }
            }
            _logger.Info("Waiting mirai connection");
            foreach (var task in tasks)
                await task;
            _logger.Info("Mirai init finished");
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
            _botManager.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
