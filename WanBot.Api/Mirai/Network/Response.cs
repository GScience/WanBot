using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Network
{
    public class WsAdapterResponse<T> : WsAdapterResponse
    {
        public T? Data { get; set; }
    }

    public class WsAdapterResponse
    {
        public string SyncId { get; set; } = string.Empty;
    }

    public class Response<ResponseContent> : Response
    {
        public ResponseContent? Data { get; set; }
    }

    public abstract class Response : IResponse
    {
        public int Code { get; set; } = -1;
        public string Msg { get; set; } = string.Empty;
    }

    public interface IResponse
    {
    }

    /// <summary>
    /// 响应码
    /// </summary>
    public static class ResponseCode
    {
        private static Dictionary<int, string> _responseDict = new()
        {
            { 0, "正常" },
            { 1, "错误的verify key" },
            { 2, "指定的Bot不存在" },
            { 3, "Session失效或不存在" },
            { 4, "Session未认证(未激活)" },
            { 5, "发送消息目标不存在(指定对象不存在)" },
            { 6, "指定文件不存在，出现于发送本地图片" },
            { 10, "无操作权限，指Bot没有对应操作的限权" },
            { 20, "Bot被禁言，指Bot当前无法向指定群发送消息" },
            { 30, "消息过长" },
            { 400, "错误的访问，如参数错误等" }
        };

        /// <summary>
        /// 正常
        /// </summary>
        public const int Ok = 0;

        /// <summary>
        /// 原因
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string Reason(int id)
        {
            if (_responseDict.TryGetValue(id, out var reason))
                return reason;
            return "未知错误";
        }
    }
}
