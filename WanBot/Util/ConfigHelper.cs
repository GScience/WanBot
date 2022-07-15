using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WanBot.Util
{
    /// <summary>
    /// 配置助手
    /// </summary>
    internal static class ConfigHelper
    {
        private static JsonSerializerOptions _options;

        static ConfigHelper()
        {
            _options = new()
            {
                WriteIndented = true,
                AllowTrailingCommas = true
            };
        }

        /// <summary>
        /// 读取配置，若不存在则创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static T ReadConfigFromFile<T>(string fullPath) where T : new()
        {
            if (File.Exists(fullPath))
            {
                using var ifs = File.OpenRead(fullPath);
                return JsonSerializer.Deserialize<T>(ifs)!;
            }

            var conf = new T();
            using var ofs = File.Create(fullPath);
            JsonSerializer.Serialize(ofs, conf, _options);
            return conf;
        }
    }
}
