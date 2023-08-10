using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Network;

namespace WanBot.Api.Mirai.Adapter
{
    /// <summary>
    /// 适配器通用接口
    /// </summary>
    public interface IAdapter : IDisposable
    {
        public string BaseUrl { get; }

        public string VerifyKey { get; }

        public long QQ { get; }

        public bool IsConnected { get; }

        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="RequestPayload"></typeparam>
        /// <typeparam name="ResponsePayload"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ResponsePayload?> SendAsync<ResponsePayload, RequestPayload>(RequestPayload? request)
            where RequestPayload : class
            where ResponsePayload : IResponse;

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <returns></returns>
        public Task ConnectAsync();
    }
}
