using WanBot.Api;

namespace WanBot.Plugin.WanCoin
{
    public class WanCoinPlugin : WanBotPlugin
    {
        public override string PluginName => "WanCoin";

        public override string PluginDescription => "虚犊币插件，水的越多币越多~";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");
    }
}