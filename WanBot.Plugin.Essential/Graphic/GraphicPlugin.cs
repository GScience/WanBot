using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Graphic;
using WanBot.Graphic.UI.Layout;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.Essential.Graphic
{
    public partial class GraphicPlugin : WanBotPlugin, IDisposable
    {
        public override string PluginName => "Graphic";

        public override string PluginAuthor => "WanNeng"; 
        
        public override string PluginDescription => "渲染插件，提供对WanBot.Graphic库的支持";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private GraphicConfig _config = null!;

        /// <summary>
        /// 渲染器
        /// </summary>
        public UIRenderer Renderer { get; set; } = null!;

        public override void PreInit()
        {
            base.PreInit();
            Renderer = new();

            try
            {
                Renderer.EnableGPU();
            }
            catch (Exception e)
            {
                Logger.Warn($"Failed to enable GPU accelleration because {e}");
            }

            _config = GetConfig<GraphicConfig>();
            LoadFontAsync().Wait();
        }

        public async Task LoadFontAsync()
        {
            var fontPath = Path.Combine(GetConfigPath(), "default.ttf");

            if (!File.Exists(fontPath))
                await DownloadFont(_config.DefaultFontUrl, fontPath);

            Fonts.LoadDefault(fontPath);
        }

        public async Task DownloadFont(string url, string saveTo)
        {
            Logger.Info("Download font from {url}", url);

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                Logger.Error("Failed to download font");

            var stream = await response.Content.ReadAsStreamAsync();
            using var fontFile = File.Create(saveTo);
            stream.CopyTo(fontFile);

            Logger.Info("Downloaded");
        }


        [Command("渲染测试")]
        public async Task OnExampleCommand(MiraiBot bot, CommandEventArgs e)
        {
            if (!e.Sender.HasPermission(this, "test"))
                return;

            var builder = new MessageBuilder();
            using var avatar = await Avatar.FromQQAsync(e.Sender.Id);
            using var ui = GetExample(e.Sender.DisplayName, avatar.Image);
            using var img = Renderer.Draw(ui);
            builder.Image(new MiraiImage(bot, img));
            await e.Sender.ReplyAsync(builder);
        }

        public void Dispose()
        {
            Renderer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
