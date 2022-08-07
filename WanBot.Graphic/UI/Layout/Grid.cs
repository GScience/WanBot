using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Graphic.UI.Layout
{
    public class Grid : UIContainer
    {
        public override SKRect UpdateLayout(SKRect rect)
        {
            base.UpdateLayout(rect);
            foreach (var child in Children)
                child.UpdateLayout(RenderRect);
            return ContentRect;
        }
        public override void DrawDebug(SKCanvas canvas)
        {
            base.DrawDebug(canvas);
            foreach (var child in Children)
                child.DrawDebug(canvas);
        }
    }
}
