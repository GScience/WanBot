using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;

namespace WanBot
{
    /// <summary>
    /// 完犊子Bot配置
    /// </summary>
    public class WanBotConfig
    {
        public List<MiraiConfig> MiraiConfigs { get; set; } = new()
        {
            new MiraiConfig()
        };

        public bool EnableAutoReload { get; set; } = false;
    }
}
