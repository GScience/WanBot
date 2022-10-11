using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Plugin.Jrrp;
using WanBot.Api;

namespace WanBot.Plugin.HelloWorld
{
    /// <summary>
    /// 与Jrrp插件的联动
    /// </summary>
    public class JrrpAddition
    {
        private JrrpPlugin _jrrpPlugin;

        public JrrpAddition(BasePlugin plugin)
        {
            _jrrpPlugin = plugin.Application.PluginManager.GetPlugin<JrrpPlugin>()!;
            _jrrpPlugin.Tips.Add((id, jrrp) =>
            {
                if (jrrp > 0.7)
                    return "完犊子今天非常开心！他会尽可能带给你好运的。\t";
                else if (jrrp > 0.5)
                    return "这很罕见。完犊子今天很冷漠。";
                else
                    return "完犊子十分不满。他会竭尽全力给你捣蛋的。\t";
            });
        }

        /// <summary>
        /// 是否能打完犊子
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CanBeatWanBotAsync(long id)
        {
            return await _jrrpPlugin?.GetJrrpAsync(id)! > 0.7;
        }
    }
}
