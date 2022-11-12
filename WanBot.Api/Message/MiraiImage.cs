using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;

namespace WanBot.Api.Message
{
    /// <summary>
    /// Mirai图像
    /// </summary>
    public class MiraiImage : IDisposable
    {
        private MiraiBot _bot;
        private SKImage _image;
        private (string url, string id)[] _imageId = new (string url, string id)[3];

        private bool _disposeImage;

        public MiraiImage(MiraiBot bot, SKImage image, bool disposeImage = true)
        {
            _bot = bot;
            _image = image;
            _disposeImage = disposeImage;
        }

        /// <summary>
        /// 获取图像Url
        /// </summary>
        /// <returns></returns>
        public async Task<(string url, string id)> SendImageAsync(MessageType imageType)
        {
            if (!string.IsNullOrEmpty(_imageId[(int)imageType].id))
                return _imageId[(int)imageType];

            var type = imageType.ToString().ToLower();
            using var data = _image.Encode(SKEncodedImageFormat.Png, 100);
            var uploadImageResponse = await _bot.UploadImageAsync(type, data.AsStream());

            _imageId[(int)imageType].id = uploadImageResponse.ImageId;
            _imageId[(int)imageType].url = uploadImageResponse.Url;

            return _imageId[(int)imageType];
        }

        public void Dispose()
        {
            if (_disposeImage)
                _image?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
