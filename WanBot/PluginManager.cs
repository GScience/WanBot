﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;

namespace WanBot
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        public BotDomain BotDomain { get; }

        public PluginLoadContext(BotDomain botDomain) : base(true)
        {
            BotDomain = botDomain;
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (Default.Assemblies.Where(
                asm => assemblyName.Name == asm.GetName().Name).Any())
                return null;
            return base.Load(assemblyName);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string platform, architecture;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                platform = "win";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platform = "linux";
            }
            else throw new PlatformNotSupportedException("Unsupported OS platform");

            if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                architecture = "x64";
            }
            else if (RuntimeInformation.ProcessArchitecture == Architecture.X86)
            {
                architecture = "x86";
            }
            else throw new PlatformNotSupportedException("Unsupported OS platform");

            var cwd = Path.Combine(Environment.CurrentDirectory, unmanagedDllName);
            var pluginDir = Path.Combine(BotDomain.CurrentApplication.PluginPath, unmanagedDllName);
            var runtimeDir = Path.Combine(BotDomain.CurrentApplication.PluginPath, "runtimes", $"{platform}-{architecture}" , "native", unmanagedDllName);

            if (File.Exists(runtimeDir))
                return NativeLibrary.Load(runtimeDir);
            else if (File.Exists(pluginDir))
                return NativeLibrary.Load(pluginDir);
            else if (File.Exists(cwd))
                return NativeLibrary.Load(cwd);
            else return base.LoadUnmanagedDll(unmanagedDllName);
        }
    }

    public class PluginManager : IPluginManager, IDisposable
    {
        private BotDomain _domain;
        private PluginLoadContext _pluginLoadContext;
        private ILogger _logger = new Logger("PluginManager");

        private PluginChangeListener? _pluginChangeListener;

        /// <summary>
        /// 插件列表
        /// </summary>
        public List<BasePlugin> Plugins { get; } = new();

        /// <summary>
        /// 加载的程序集列表
        /// </summary>
        public List<(Assembly asm, string path)> AsmList { get; } = new();

        public PluginManager(BotDomain domain)
        {
            _domain = domain;
            _pluginLoadContext = new(domain);
            try
            {
                LoadInternalDeps();
            }
            catch (Exception e)
            {
                _logger.Error("Failed to load internal deps because {e}", e);
            }
        }

        /// <summary>
        /// 卸载插件
        /// </summary>
        private void UnloadPlugins()
        {
            foreach (var plugin in Plugins)
                plugin.Stop();
            foreach (var plugin in Plugins)
            {
                plugin.Logger.Info("Unload plugin {name}", plugin.PluginName);

                try
                {
                    if (plugin is IDisposable disposable)
                        disposable.Dispose();
                    else if (plugin is IAsyncDisposable asyncDisposable)
                        asyncDisposable.DisposeAsync().AsTask().Wait();
                }
                catch (Exception e)
                {
                    plugin.Logger.Info("Failedd to unload plugin because {e}", e);
                }
            }

            Plugins.Clear();
            AsmList.Clear();
        }

        /// <summary>
        /// 加载内部依赖
        /// </summary>
        public void LoadInternalDeps()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            _logger.Info("Load deps from {exePath}", basePath);

            foreach (var file in Directory.EnumerateFiles(basePath, "*.dll"))
            {
                var assemblyName = AssemblyName.GetAssemblyName(file);

                if (!AssemblyLoadContext.Default.Assemblies.Where(
                    asm => assemblyName.Name == asm.GetName().Name).Any())
                        Assembly.LoadFrom(file);
            }
        }

        public void Reload(bool unloadContext)
        {
            UnloadPlugins();

            if (unloadContext)
            {
                _logger.Warn("Unload plugin context can cause memory problem");

                _pluginLoadContext.Unload();
                _pluginLoadContext = new PluginLoadContext(_domain);
            }
        }

        public void LoadAssemblysFromDir(string dir)
        {
            AsmList.Add((typeof(WanBotPlugin).Assembly, string.Empty));
            AsmList.Add((typeof(PluginManager).Assembly, string.Empty));

            if (_domain.CurrentApplication.Config.EnableAutoReload)
            {
                _pluginChangeListener = new PluginChangeListener(dir);
                _pluginChangeListener.OnPluginChange += s => _domain.CurrentApplication.Reload(false, true);
            }
            else
            {
                _pluginChangeListener?.Dispose();
                _pluginChangeListener = null;
            }

            foreach (var file in Directory.EnumerateFiles(dir, "*.dll"))
            {
                var fileInfo = new FileInfo(file);

                _logger.Info("Loading {file}", fileInfo.Name);
                try
                {
                    var assemblyName = AssemblyName.GetAssemblyName(file);

                    if (AssemblyLoadContext.Default.Assemblies.Where(
                        asm => assemblyName.Name == asm.GetName().Name).Any())
                        throw new Exception($"{assemblyName.Name} has already loaded");

                    using var stream = File.OpenRead(file);
                    var asm = _pluginLoadContext.LoadFromStream(stream);
                    AsmList.Add((asm, file));
                }
                catch (Exception e)
                {
                    _logger.Error("Failed to load {file} because: \n{e}", fileInfo.Name, e);
                }
            }
        }

        public void EnumPlugins()
        {
            foreach (var (asm, _) in AsmList)
            {
                try
                {
                    EnumPluginInAssembly(asm);
                }
                catch (Exception e)
                {
                    _logger.Error("Failed to enum plugin type in assembly {asm} because:\n{e}", asm, e);
                }
            }
        }

        public void EnumPluginInAssembly(Assembly asm)
        {
            foreach (var type in asm.GetTypes())
            {
                if (!typeof(BasePlugin).IsAssignableFrom(type) ||
                    type.IsAbstract)
                    continue;

                try
                {
                    LoadPluginFromType(type);
                }
                catch (Exception e)
                {
                    _logger.Error($"Failed to load plugin from type {type.Name}\n{e}", e);
                }
            }
        }

        private void LoadPluginFromType(Type type)
        {
            _logger.Info("Find {PluginName} in {AsmName}", type.Name, type.Assembly.GetName().Name);
            var pluginLogger = new Logger(type.Name);
            var plugin = BasePlugin.CreatePluginInstance(type, _domain.CurrentApplication, pluginLogger);
            pluginLogger.SetCategory(plugin.PluginName);
            _logger.Info(
                "Plugin {PluginName} by {PluginAuthor} version {PluginVersion}",
                plugin.PluginName,
                plugin.PluginAuthor,
                plugin.PluginVersion);

            try
            {
                plugin.PreInit();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed while PreInit plugin {plugin.GetType().Name}. Plugin not load.", e);
            }

            Plugins.Add(plugin);
        }

        public void InitPlugins()
        {
            _logger.Info("Init Stage");
            foreach (var plugin in Plugins)
            {
                try
                {
                    plugin.Init();
                }
                catch
                {
                    _logger.Error("Failed while Init plugin {PluginName}", plugin.GetType().Name);
                }
            }
            _logger.Info("Init Stage Finished");
        }

        public void PostInitPlugins()
        {
            _logger.Info("PostInit Stage");
            foreach (var plugin in Plugins)
            {
                try
                {
                    plugin.PostInit();
                }
                catch (Exception e)
                {
                    _logger.Error("Failed while PostInit plugin {PluginName}\n{e}", plugin.GetType().Name, e);
                }
            }
            _logger.Info("PostInit Stage Finished");
        }
        public void StartPlugins()
        {
            _logger.Info("Start Stage");
            foreach (var plugin in Plugins)
            {
                try
                {
                    plugin.Start();
                }
                catch (Exception e)
                {
                    _logger.Error("Failed while PostInit plugin {PluginName}\n{e}", plugin.GetType().Name, e);
                }
            }
            _logger.Info("Start Stage Finished");
        }
        
        public T? GetPlugin<T>() where T : BasePlugin
        {
            foreach (var plugin in Plugins)
                if (plugin is T result)
                    return result;
            return null;
        }

        public string GetPluginPath(BasePlugin plugin)
        {
           return GetAssemblyPath(plugin.GetType().Assembly);
        }

        public string GetAssemblyPath(Assembly pluginAsm)
        {
            var pluginPath = AsmList.Where((pair) => pair.asm == pluginAsm).FirstOrDefault();
            return pluginPath.path;
        }

        public void Dispose()
        {
            UnloadPlugins();
            _pluginChangeListener?.Dispose();
        }
    }
}
