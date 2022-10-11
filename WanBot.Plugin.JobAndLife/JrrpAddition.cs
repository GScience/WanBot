using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Plugin.Jrrp;
using WanBot.Api;

namespace WanBot.Plugin.JobAndLife
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
                if (GetJrrpSalaryScale(jrrp) > 1)
                    return "今天非常适合#打工哦！老板看好你~";
                else if (GetJrrpSalaryScale(jrrp) > 0.8)
                    return "今天有一点不在状态，是不是不适合#打工呢";
                else
                    return "今天完全不想#打工";
            });
        }

        /// <summary>
        /// 获取Jrrp系数
        /// </summary>
        /// <returns></returns>
        private async Task<float> GetJrrpAsync(long id)
        {
            return await _jrrpPlugin?.GetJrrpAsync(id)!;
        }

        /// <summary>
        /// 根据Jrrp计算工资系数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<float> GetJrrpSalaryScale(long id)
        {
            return GetJrrpSalaryScale(await GetJrrpAsync(id));
        }

        /// <summary>
        /// 根据Jrrp计算工资系数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private float GetJrrpSalaryScale(float jrrp)
        {
            return jrrp * 0.5f + 0.6f;
        }
    }
}
