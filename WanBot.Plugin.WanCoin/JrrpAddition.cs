using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Plugin.Jrrp;
using WanBot.Api;

namespace WanBot.Plugin.WanCoin
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
                var wanCoinPlugin = plugin as WanCoinPlugin;
                if (wanCoinPlugin == null)
                    return "虚度币呢？？！";
                using var wanCoinDb = wanCoinPlugin.GetWanCoinDatabase();
                var serverUser = wanCoinPlugin.GetWanCoinUser(wanCoinDb, WanCoinPlugin.ServerQQId);
                var buyPrice = wanCoinPlugin.GetCurrentPrice(serverUser.CoinCount - 1);
                var sellPrice = wanCoinPlugin.GetCurrentPrice(serverUser.CoinCount);
                if (jrrp < 0.5)
                    return $"虚犊币当前价格 买：{buyPrice} 卖：{sellPrice}（快买 快买 快买）";
                else
                    return $"虚犊币当前价格 买：{buyPrice} 卖：{sellPrice}（快卖 快卖 快卖）";
            });
        }
    }
}
