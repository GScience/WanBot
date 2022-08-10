using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Graphic;
using WanBot.Graphic.UI.Layout;

namespace WanBot.Plugin.Essential.Graphic
{
    public class GraphicPlugin : WanBotPlugin
    {
        public override string PluginName => "Graphic";

        public override string PluginAuthor => "WanNeng"; 
        
        public override string PluginDescription => "渲染插件，提供对WanBot.Graphic库的支持";

        public override Version PluginVersion => Version.Parse("1.0.0");

        public override void PreInit()
        {
            base.PreInit();

            var grid = new Grid(); 
            var rect = grid.UpdateLayout(new SKRect(0, 0, 2000, 300));
            var imageInfo = new SKImageInfo((int)rect.Width, (int)rect.Height);
            using var surface = SKSurface.Create(VkContext.Current.GrContext, false, imageInfo);
            grid.Draw(surface.Canvas);
        }
    }
}
