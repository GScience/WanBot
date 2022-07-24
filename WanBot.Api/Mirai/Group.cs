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
    public struct Group
    {
        public Group()
        {
        }

        /// <summary>
        /// 群号
        /// </summary>
        public long Id { get; set; } = 0;

        /// <summary>
        /// 群名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Bot在群内的权限
        /// </summary>
        public string Permission { get; set; } = string.Empty;

        public string GetFormatedName()
        {
            return $"[{Name}({Id})]";
        }
    }
}
