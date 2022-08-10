using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.YGO
{
    public static class YgoCardImage
    {
        public static string CachePath = string.Empty;

        /// <summary>
        /// 获取图像下载链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string GetCardImageUrl(int id)
        {
            return $"https://cdn02.moecube.com:444/ygomobile-images/{id}.png";
        }

        public static async Task<string?> GetCardImagePathAsync(int id)
        {
            if (!string.IsNullOrEmpty(CachePath) && 
                !Directory.Exists(CachePath))
                Directory.CreateDirectory(CachePath);
            var path = Path.Combine(CachePath, $"{id}.png");
            if (string.IsNullOrEmpty(CachePath) || !File.Exists(path))
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(GetCardImageUrl(id));
                if (!response.IsSuccessStatusCode)
                    return null;

                using var data = SKData.Create(response.Content.ReadAsStream());
                using var cacheStream = File.Create(path);
                data.SaveTo(cacheStream);
            }
            return path;
        }

        public static async Task<SKImage?> LoadFromIdAsync(int id)
        {
            var path = await GetCardImagePathAsync(id);
            if (string.IsNullOrEmpty(path))
                return null;
            using var data = SKData.Create(path);
            return SKImage.FromEncodedData(data);
        }
    }
}
