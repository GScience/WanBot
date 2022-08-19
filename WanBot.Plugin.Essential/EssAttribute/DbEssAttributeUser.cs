using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.Essential.EssAttribute
{
    public class DbEssAttributeUser
    {
        /// <summary>
        /// QQ号
        /// </summary>
        public long Id { get; internal set; }

        /// <summary>
        /// 钱
        /// </summary>
        public string Money { get; set; } = "0";

        /// <summary>
        /// 最大精力上限，默认100
        /// </summary>
        public int EnergyMax { get; set; } = 100;

        /// <summary>
        /// 当前精力
        /// </summary>
        public int Energy { get; set; }

        /// <summary>
        /// 上一次查询能量的时间
        /// </summary>
        public DateTime LastTimeCheckEnergy { get; set; }

        /// <summary>
        /// 额外攻击力
        /// </summary>
        public int AttactAddition { get; set; }

        /// <summary>
        /// 额外防御力
        /// </summary>
        public int DefenceAddition { get; set; }

        /// <summary>
        /// 额外特攻
        /// </summary>
        public int SpAttactAddition { get; set; }

        /// <summary>
        /// 额外特防
        /// </summary>
        public int SpDefenceAddition { get; set; }

        /// <summary>
        /// 额外血量
        /// </summary>
        public int HpAddition { get; set; }

        /// <summary>
        /// 额外魔力值
        /// </summary>
        public int MagicAddition { get; set; }
    }
}
