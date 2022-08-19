using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.Jrrp
{
    public class JrrpUser
    {
        /// <summary>
        /// QQ号
        /// </summary>
        public long Id { get; internal set; }

        /// <summary>
        /// 今日运势
        /// </summary>
        public float Jrrp { get; set; }

        /// <summary>
        /// 能做的事情Id
        /// </summary>
        public int CanDo { get; set; }

        /// <summary>
        /// 不能做的事情Id
        /// </summary>
        public int CantDo { get; set; }

        /// <summary>
        /// 上次获取日期
        /// </summary>
        public DateTime LastTime { get; set; }
    }
}
