using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.Jrrp
{
    public class JrrpConfig
    {
        public List<string> Activity { get; set; } = new()
        {
            "打牌",
            "打音游",
            "写代码",
            "女装",
            "学习",
            "修仙",
            "水群",
            "考试",
            "打工",
            "逛街",
            "跑步",
            "氪金",
            "抽卡"
        };
    }
}
