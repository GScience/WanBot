using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api
{
    /// <summary>
    /// 插件管理器接口
    /// </summary>
    public interface IPluginManager
    {
        /// <summary>
        /// 获取指定的插件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T? GetPlugin<T>() where T : BasePlugin;
    }
}
