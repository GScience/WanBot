using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai
{
    /// <summary>
    /// 群组
    /// </summary>
    public class Group
    {
        /// <summary>
        /// 群号
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 群名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Bot在群内的权限
        /// </summary>
        public string Permission { get; set; } = string.Empty;
    }
}
