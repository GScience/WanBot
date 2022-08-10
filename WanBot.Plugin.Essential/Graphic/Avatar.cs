using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.Essential.Graphic
{
    public class Avatar : IDisposable
    {
        public SKImage Image { get; }

        private Avatar(SKImage? image)
        {
            Image = image;
        }

        public static async Task<Avatar> FromQQAsync(long qq)
        {
            using var httpClient = new HttpClient();

            var url = $"http://q1.qlogo.cn/g?b=qq&nk={qq}&s=640";
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new Avatar(null);

            var skData = SKData.Create(await response.Content.ReadAsStreamAsync());
            var image = SKImage.FromEncodedData(skData);

            return new Avatar(image);
        }

        public void Dispose()
        {
            Image?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
