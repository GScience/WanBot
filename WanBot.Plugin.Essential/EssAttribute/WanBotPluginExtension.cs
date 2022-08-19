using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Plugin.Essential.Extension;

namespace WanBot.Plugin.Essential.EssAttribute
{
    public static class WanBotPluginExtension
    {
        public static EssAttrUserFactory GetEssAttrUserFactory(this WanBotPlugin plugin)
        {
            return plugin.Application.PluginManager.GetPlugin<EssAttributePlugin>()?.essAttrUsrFactory
                ?? throw new Exception("Failed to get essAttrUsrFactory");
        }
    }
}
