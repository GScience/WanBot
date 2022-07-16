using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Network;

namespace WanBot.Api.Mirai.Payload
{
    [HttpApi("uploadImage", HttpAdapterMethod.PostMultipart)]
    public class UploadImageRequest
    {
        public string SessionKey { get; set; } = string.Empty;
        
        /// <summary>
        /// friend或group或temp
        /// </summary>
        public string Type { get; set; } = string.Empty;

        public Stream Img { get; set; } = null!;
    }

    public class UploadImageResponse : IResponse
    {
        public string ImageId { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
    }
}
