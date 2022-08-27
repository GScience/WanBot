using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.RandomStuff
{
    public class RandomStuffConfig
    {
        public string[] TalkWords { get; set; } =
        {
            "？","你说啥？","嗷？", "我不", "哼！"
        };

        /// <summary>
        /// 概率基数
        /// </summary>
        public int RateBase { get; set; } = 20;
    }
}
