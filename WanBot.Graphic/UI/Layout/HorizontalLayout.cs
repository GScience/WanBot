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

        /// <summary>
        /// 水平布局
        /// </summary>
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;

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
                SKRect childRect;

                var childHeight = child.Height;

                if (childHeight == null && !(child.Margin.Top != null && child.Margin.Bottom != null))
                    childHeight = child.RenderRect.Height;

                if (childHeight != null && childHeight > 0)
                {
                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Center:
                            {
                                var layoutHeight = layoutBottom - currentPosY - child.Margin.Top ?? 0;
                                var layoutOffset = (layoutHeight - childHeight) / 2;

                                var layoutOffsetTop = layoutOffset - child.Margin.Top ?? 0;
                                var layoutOffsetBottom = layoutOffset - child.Margin.Bottom ?? 0;
                                childRect = new SKRect(currentPosX, currentPosY + layoutOffsetTop, currentPosX + ChildWidth, layoutBottom - layoutOffsetBottom);
                                break;
                            }
                        case HorizontalAlignment.Bottom:
                            {
                                var layoutHeight = layoutBottom - currentPosY - child.Margin.Top ?? 0;
                                var layoutOffset = layoutHeight - childHeight;

                                var layoutOffsetTop = layoutOffset - child.Margin.Top ?? 0;
                                var layoutOffsetBottom = layoutOffset - child.Margin.Bottom ?? 0;
                                childRect = new SKRect(currentPosX, currentPosY + layoutOffsetTop, currentPosX + ChildWidth, layoutBottom - layoutOffsetBottom);
                                break;
                            }
                        default:
                            childRect = new SKRect(currentPosX, currentPosY, currentPosX + ChildWidth, layoutBottom);
                            break;
                    }
                }
                else
                {
                    childRect = new SKRect(currentPosX, currentPosY, currentPosX + ChildWidth, layoutBottom);
                }
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
