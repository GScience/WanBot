using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api
{
    public class WanBotApi : WanBotPlugin
    {
        public override string PluginName => "WanBot.Api";

        public override string PluginAuthor => "WanNeng";

        public override string PluginDescription => "完犊子Api，提供通用化的QQ机器人接口与mirai http协议的实现";

        public override Version PluginVersion => GetType().Assembly.GetName().Version ?? Version.Parse("1.0.0");
    }
}
