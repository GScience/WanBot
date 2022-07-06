using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai
{
    /// <summary>
    /// 好友
    /// </summary>
    public struct Friend
    {
        public Friend()
        {
        }

        /// <summary>
        /// QQ号
        /// </summary>
        public long Id { get; set; } = 0;

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; } = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = string.Empty;
    }
}
