using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Graphic.UI.Layout
{
    /// <summary>
    /// 水平布局
    /// </summary>
    public class HorizontalLayout : UIContainer
    {
        /// <summary>
        /// 布局的间隙
        /// </summary>
        public float Space { get; set; }

        /// <summary>
        /// 子元素高度，不保证所有子元素的宽度均为该数值
        /// </summary>
        public float ChildWidth { get; set; }

        public override void DrawDebug(SKCanvas canvas)
        {
            base.DrawDebug(canvas);
            foreach (var child in Children)
                child.DrawDebug(canvas);
        }

        public override SKRect UpdateLayout(SKRect inputRect)
        {
            var rect = base.UpdateLayout(inputRect);

            var currentPosY = rect.Top;
            var currentPosX = rect.Left;
            var layoutBottom = Math.Max(rect.Top + Height ?? 0f, inputRect.Bottom);

            // 扩张容器
            foreach (var child in Children)
            {
                var childRect = new SKRect(currentPosX, currentPosY, currentPosX + ChildWidth, layoutBottom);
                childRect = child.UpdateLayout(childRect);
                layoutBottom = Math.Max(childRect.Bottom, layoutBottom);
                currentPosX = Space + childRect.Width + childRect.Left;
            }

            currentPosX = rect.Left;

            // 布局
            foreach (var child in Children)
            {
                var childRect = new SKRect(currentPosX, currentPosY, currentPosX + ChildWidth, layoutBottom);
                childRect = child.UpdateLayout(childRect);
                currentPosX = Space + childRect.Width + childRect.Left;
            }

            RenderRect = new SKRect(rect.Left, rect.Top, currentPosX, layoutBottom);

            var uiRect = RenderRect;
            if (Margin.Right != null)
                uiRect.Right += Margin.Right.Value;
            if (Margin.Left != null)
                uiRect.Right += Margin.Left.Value;
            if (Margin.Top != null)
                uiRect.Bottom += Margin.Top.Value;
            if (Margin.Bottom != null)
                uiRect.Bottom += Margin.Bottom.Value;
            ContentRect = uiRect;

            return ContentRect;
        }
    }
}
