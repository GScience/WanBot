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
        private string[] _imageId = new string[3];

        private bool _disposeImage;

        public MiraiImage(MiraiBot bot, SKImage image, bool disposeImage = true)
        {
            _bot = bot;
            _image = image;
            _disposeImage = disposeImage;
        }

        /// <summary>
        /// 获取图像Id
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetImageIdAsync(MessageType imageType)
        {
            if (!string.IsNullOrEmpty(_imageId[(int)imageType]))
                return _imageId[(int)imageType];

            var type = imageType.ToString().ToLower();
            using var data = _image.Encode(SKEncodedImageFormat.Png, 100);
            var uploadImageResponse = await _bot.UploadImageAsync(type, data.AsStream());

            return uploadImageResponse.ImageId;
        }

        public void Dispose()
        {
            if (_disposeImage)
                _image?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
