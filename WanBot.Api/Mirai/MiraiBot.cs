using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;

namespace WanBot.Api.Mirai
{
    /// <summary>
    /// Mirai 机器人
    /// </summary>
    public class MiraiBot
    {
        private Dictionary<Type, IAdapter> _adapterDict = new();

        public bool IsConnected { get; private set; } = false;

        /// <summary>
        /// 会话Key
        /// </summary>
        public string SessionKey { get; private set; } = string.Empty;

        /// <summary>
        /// 连接到Mirai
        /// </summary>
        /// <param name="config"></param>
        public void Connect(MiraiConfig config)
        {
            if (IsConnected)
                throw new Exception("Mirai bot already connect to mirai");



            IsConnected = true;
        }

        /// <summary>
        /// 获取适配器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetAdapter<T>() where T : class, IAdapter
        {
            if (_adapterDict.TryGetValue(typeof(T), out var adapter))
                return adapter as T;
            return null;
        }
    }
}
