using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Graphic.UI
{
    public class Rectangle : UIElement
    {
        /// <summary>
        /// 边框圆角
        /// </summary>
        public SKSize Radius { get; set; } = new()
        {
            Height = 10,
            Width = 10
        };

        /// <summary>
        /// 边框
        /// </summary>
        public float Border { get; set; }

        public SKPaint Paint { get; set; } = new()
        {
            Color = SKColors.White,
            IsAntialias = true
        };

        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawRoundRect(RenderRect, Radius, Paint);
        }

        public override void DrawDebug(SKCanvas canvas)
        {
            base.DrawDebug(canvas);
        }
    }
}
