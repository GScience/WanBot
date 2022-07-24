using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;

namespace WanBot
{
    public class PluginManager : IPluginManager
    {
        private ILogger _logger = new Logger("PluginManager");

        private List<BasePlugin> _plugins = new();

        public void LoadAssemblysFromDir(string dir)
        {
            foreach (var file in Directory.EnumerateFiles(dir, "*.dll"))
            {
                _logger.Info("Loading {file}", file);
                try
                {
                    Assembly.LoadFile(file);
                }
                catch (Exception e)
                {
                    _logger.Info("Failed to load {file} because: \n{e}", file, e);
                }
            }
        }

        public void EnumPlugins()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
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
                    _logger.Error($"Failed to load plugin from type {type.Name}", e);
                }
            }
        }

        private void LoadPluginFromType(Type type)
        {
            _logger.Info("Find {PluginName}.", type.Name);
            var pluginLogger = new Logger(type.Name);
            var plugin = BasePlugin.CreatePluginInstance(type, Application.Current, pluginLogger);
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

            _plugins.Add(plugin);
        }

        public void InitPlugins()
        {
            _logger.Info("Init Stage");
            foreach (var plugin in _plugins)
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
            foreach (var plugin in _plugins)
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
            foreach (var plugin in _plugins)
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
            foreach (var plugin in _plugins)
                if (plugin is T result)
                    return result;
            return null;
        }
    }
}
