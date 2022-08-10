using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Graphic.UI
{
    public class ImageBox : UIElement
    {
        public static SKPaint Empty { get; set; } = new()
        {
            Color = SKColors.White
        };

        /// <summary>
        /// 图像
        /// </summary>
        public SKImage? Image { get; set; }

        public override void Draw(SKCanvas canvas)
        {
            if (Image == null)
                canvas.DrawRect(RenderRect, Empty);
            else
                canvas.DrawImage(Image, RenderRect);
        }

        public override SKRect UpdateLayout(SKRect rect)
        {
            return base.UpdateLayout(rect);
        }
    }
}
