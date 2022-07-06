using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Util
{
    /// <summary>
    /// 生成同步Id
    /// </summary>
    internal static class SyncIdHelper
    {
        private static uint _offset;

        public static string Next()
        {
            ++_offset;
            return $"{DateTime.Now.ToFileTimeUtc()}{_offset}";
        }
    }
}
