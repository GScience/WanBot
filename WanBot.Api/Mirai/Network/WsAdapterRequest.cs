using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;

namespace WanBot.Api.Mirai.Network
{
    /// <summary>
    /// Ws适配器请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class WsAdapterRequest<T>
    {
        private static string _command { get; set; }
        private static string _subCommand { get; set; }

        static WsAdapterRequest()
        {
            var apiAttr = typeof(T).GetCustomAttribute<WsApiAttribute>();

            if (apiAttr == null)
                throw new Exception($"{typeof(T)} is not a ws api payload");

            _command = apiAttr.Command;
            _subCommand = apiAttr.SubCommand;
        }

        public string SyncId { get; set; }
        public string Command => _command;
        public string SubCommand => _subCommand;
        public T? Content { get; set; }

        public WsAdapterRequest(string syncId, T? content)
        {
            SyncId = syncId;
            Content = content;
        }
    }
}
