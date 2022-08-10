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
            var result = base.UpdateLayout(rect);
            var right = result.Right;
            var bottom = result.Bottom;

            // 测量布局
            foreach (var child in Children)
            {
                var childRect = child.UpdateLayout(RenderRect);
                right = Math.Max(childRect.Right, right);
                bottom = Math.Max(childRect.Bottom, bottom);
            }

            var oldWidth = Width;
            var oldHeight = Height;
            Width = right - result.Left;
            Height = bottom - result.Top;

            result = base.UpdateLayout(rect);
            Width = oldWidth;
            Height = oldHeight;

            // 布局
            foreach (var child in Children)
                child.UpdateLayout(RenderRect);

            return result;
        }
        public override void DrawDebug(SKCanvas canvas)
        {
            base.DrawDebug(canvas);
            foreach (var child in Children)
                child.DrawDebug(canvas);
        }
    }
}
