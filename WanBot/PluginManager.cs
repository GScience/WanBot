using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;

namespace WanBot
{
    public class PluginManager
    {
        private ILogger _logger = new Logger("PluginManager");

        private List<BasePlugin> _plugins = new();

        public void LoadAssemblysFromDir(string dir)
        {

        }

        public void FindPlugins()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in asm.GetTypes())
                {
                    if (!typeof(BasePlugin).IsAssignableFrom(type) ||
                        type.IsAbstract)
                        continue;

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
                        _logger.Error("Failed while PreInit plugin {PluginName}. Plugin not load.\n{ex}", plugin.GetType().Name, e);
                        continue;
                    }
                    _plugins.Add(plugin);
                }
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
                catch
                {
                    _logger.Error("Failed while PostInit plugin {PluginName}", plugin.GetType().Name);
                }
            }
            _logger.Info("PostInit Stage Finished");
        }
    }
}
