using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;

namespace WanBot
{
    public class WanBot : WanBotPlugin
    {
        public override string PluginName => "WanBot";

        public override string PluginAuthor => "WanNeng"; 
        
        public override string PluginDescription => "完犊子Bot，负责对插件进行管理以及和Mirai的连接";

        public override Version PluginVersion => GetType().Assembly.GetName().Version ?? Version.Parse("1.0.0");
    }
}
