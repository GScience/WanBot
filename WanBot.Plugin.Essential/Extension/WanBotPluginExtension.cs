using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Graphic;
using WanBot.Plugin.Essential.Graphic;

namespace WanBot.Plugin.Essential.Extension
{
    public static class WanBotPluginExtension
    {
        public static UIRenderer GetUIRenderer(this WanBotPlugin plugin)
        {
            return plugin.Application.PluginManager.GetPlugin<GraphicPlugin>()?.Renderer
                ?? throw new Exception("Failed to get renderer");
        }

        public static Scheduler GetScheduler(this WanBotPlugin plugin)
        {
            return plugin.Application.PluginManager.GetPlugin<ExtensionPlugin>()?.scheduler
                ?? throw new Exception("Failed to get scheduler");
        }

        public static BotHelp GetBotHelp(this WanBotPlugin plugin)
        {
            return plugin.Application.PluginManager.GetPlugin<ExtensionPlugin>()?.botHelp
                ?? throw new Exception("Failed to get botHelp");
        }
    }
}
