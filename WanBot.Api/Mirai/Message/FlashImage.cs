using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// 闪照消息，三个参数任选其一，出现多个参数时，按照imageId > url > path > base64的优先级
    /// </summary>
    public class FlashImage : BaseChain
    {
        /// <summary>
        /// 图片的imageId，群图片与好友图片格式不同。不为空时将忽略url属性
        /// </summary>
        public string? ImageId { get; set; } = null;

        /// <summary>
        /// 图片的URL，发送时可作网络图片的链接；接收时为腾讯图片服务器的链接，可用于图片下载
        /// </summary>
        public string? Url { get; set; } = null;

        /// <summary>
        /// 图片的路径，发送本地图片，路径相对于 JVM 工作路径（默认是当前路径，可通过 -Duser.dir=...指定），也可传入绝对路径。
        /// </summary>
        public string? Path { get; set; } = null;

        /// <summary>
        /// 图片的 Base64 编码
        /// </summary>
        public string? Base64 { get; set; } = null;
    }
}
