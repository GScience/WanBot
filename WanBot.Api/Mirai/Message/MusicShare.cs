using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WanBot.Api.Mirai.Message.Forward;

namespace WanBot.Api.Mirai.Message
{
    /// <summary>
    /// 音乐分享
    /// </summary>
    public class MusicShare : BaseChain
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Kind { get; set; } = string.Empty;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 概括
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// 跳转路径
        /// </summary>
        public string JumpUrl { get; set; } = string.Empty;

        /// <summary>
        /// 封面路径
        /// </summary>
        public string PictureUrl { get; set; } = string.Empty;

        /// <summary>
        /// 音源路径
        /// </summary>
        public string MusicUrl { get; set; } = string.Empty;

        /// <summary>
        /// 简介
        /// </summary>
        public string Brief { get; set; } = string.Empty;

        public override int GetHashCode()
        {
            return 
                Kind.GetHashCode() ^ 
                Title.GetHashCode() ^
                Summary.GetHashCode() ^ 
                JumpUrl.GetHashCode() ^ 
                PictureUrl.GetHashCode() ^ 
                Brief.GetHashCode();
        }
    }
}
