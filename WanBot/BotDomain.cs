using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WanBot.Api;

namespace WanBot
{
    public class BotDomain : IDisposable
    {
        public Application CurrentApplication { get; private set; }

        private Logger _logger;
        private string[] _args;
        private Semaphore _appCloseSemaphore = new(0, 1);
        private bool _pendingRestart = false;

        private Thread? _appThread;

        private BotDomain(string[] args)
        {
            // 监听未处理异常
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            // 显示日志
            _logger = new Logger($"Launcher");
            _logger.Info("WanBot By GScience Studio Ver {WanBotVersion} (Api: {ApiVersion})",
                typeof(Application).Assembly.GetName().Version,
                typeof(BasePlugin).Assembly.GetName().Version);
            _logger.Info(
                "System info: \n" +
                "\t.net version: {netVersion}\n" +
                "\tOS version: {osVersion}\n" +
                "\tEF Version: {efVersion}",
                Environment.Version,
                Environment.OSVersion,
                typeof(DbContext).Assembly.GetName().Version);

            _args = args;
            CurrentApplication = new(this, _args);
        }

        public void Run()
        {
            _appThread = new Thread(() =>
            {
                try
                {
                    CurrentApplication.Run();
                }
                catch (Exception e)
                {
                    _logger.Fatal(
                        $"============================\n" +
                        $"=         CRASH!!!         =\n" +
                        $"============================\n" +
                        $"Unknown exception: \n{e}");
                }

                if (!_pendingRestart)
                    _appCloseSemaphore.Release(1);
                else
                    _pendingRestart = false;
            });
            _appThread.Start();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            CurrentApplication.Dispose();
            CurrentApplication = null!;
        }

        public static void Launch(string[] args)
        {
            using var domain = new BotDomain(args);
            domain.Run();
            AssemblyLoadContext.Default.Unloading += (context) =>
            {
                domain._logger.Info("Unloading default assembly load context");
                domain._appCloseSemaphore.Release();
            };
            domain._appCloseSemaphore.WaitOne();
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Error("Unhandled exception: {e}", e.ExceptionObject);
        }
    }
}
