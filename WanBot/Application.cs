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
        public BotDomain BotDomain { get; }

        internal BotManager _botManager = new();

        /// <summary>
        /// 是否正在重启
        /// </summary>
        internal bool IsReloading { get; private set; }

        /// <summary>
        /// 账户管理器
        /// </summary>
        public IBotManager BotManager => _botManager;

        internal PluginManager _pluginManager { get; }

        /// <summary>
        /// 插件管理器
        /// </summary>
        public IPluginManager PluginManager => _pluginManager;

        /// <summary>
        /// 日志
        /// </summary>
        private Logger _logger = new Logger("WanBot");

        /// <summary>
        /// 配置路径
        /// </summary>
        public string ConfigPath { get; } = Path.Combine(Environment.CurrentDirectory, "Config");

        /// <summary>
        /// 插件路径
        /// </summary>
        public string PluginPath { get; } = Path.Combine(Environment.CurrentDirectory, "Plugin");

        /// <summary>
        /// 配置
        /// </summary>
        public WanBotConfig Config { get; private set; }

        private Semaphore _consoleCloseSemaphore = new(0, 1);
        private bool _isClosing = false;

        public Application(BotDomain domain, string[] args)
        {
            BotDomain = domain;

            // 注册取消事件
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                _logger.Info("Get console cancel requirement");
                if (!_isClosing)
                    HandleApplicationExit();
            };

            // 获取参数
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "-config")
                    ConfigPath = args[++i];

                if (args[i].ToLower() == "-plugin")
                    PluginPath = args[++i];
            }

            // 创建插件管理器
            _pluginManager = new(domain);
            Config = ReadConfigFromFile<WanBotConfig>("WanBot.conf");
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

        public T ReadConfig<T>(string pluginName) where T : new()
        {
            return ReadConfigFromFile<T>(Path.Combine(pluginName, "Config.conf"));
        }

        public string GetConfigPath(string pluginName)
        {
            return Path.Combine(ConfigPath, pluginName);
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        public void Reload(bool disconnectFromMirai, bool unloadContext)
        {
            if (IsReloading)
                return;

            IsReloading = true;
            Config = ReadConfigFromFile<WanBotConfig>("WanBot.conf");

            try
            {
                _logger.Info("Reloading");
                _botManager.Reload(disconnectFromMirai);
                _pluginManager.Reload(unloadContext);

                Init(disconnectFromMirai);
            }
            catch (Exception e)
            {
                _logger.Fatal("Failed to reload application because {e}", e);
            }
            finally
            {
                IsReloading = false;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectMirai"></param>
        internal void Init(bool reconnectToMirai)
        {
            var bindAccountTask
                = reconnectToMirai ? BindAccountAsync() : null;

            // 加载插件
            _logger.Info("Loading plugins");
            _pluginManager.LoadAssemblysFromDir(PluginPath);
            _pluginManager.EnumPlugins();
            _pluginManager.InitPlugins();

            // 等待账户绑定
            bindAccountTask?.Wait();

            _pluginManager.PostInitPlugins();
            _pluginManager.StartPlugins();
            _logger.Info("All done");
        }

        /// <summary>
        /// 启动应用
        /// </summary>
        internal void Run()
        {
            // 初始化
            Init(true);

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

            var tasks = new List<(Task task, MiraiConfig config)>();
            foreach (var miraiConfig in Config.MiraiConfigs)
                tasks.Add((_botManager.AddAccountAsync(miraiConfig), miraiConfig));

            _logger.Info("Waiting mirai connection");
            foreach (var (task, config) in tasks)
            {
                try
                {
                    await task;
                }
                catch (Exception e)
                {
                    _logger.Warn("Failed to connect to bot because {ex}", e.ToString());
                    _logger.Warn("Skip checking bot connection");
                    _ = Task.Run(async () =>
                    {
                        while (true)
                        {
                            try
                            {
                                _logger.Info("Trying to connect to bot ({qq})", config.QQ);
                                await _botManager.AddAccountAsync(config);
                                break;
                            }
                            catch(Exception ex)
                            {
                                _logger.Warn("Failed to connect to bot because {ex}", ex.ToString());
                                _logger.Error("Retry after 5 seconds...");
                                Thread.Sleep(5000);
                            }
                        }
                    });
                }
            }
            _logger.Info("Mirai init finished");
        }

        /// <summary>
        /// 处理应用程序退出事件
        /// </summary>
        public void HandleApplicationExit()
        {
            _isClosing = true;
            _logger.Info("pending close");
            _consoleCloseSemaphore.Release(1);
        }

        public void Dispose()
        {
            _logger.Info("Dispose resources");

            _consoleCloseSemaphore.Dispose();
            _botManager.Dispose();
            _pluginManager.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
