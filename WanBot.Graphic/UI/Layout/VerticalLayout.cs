using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Graphic.UI.Layout
{
    /// <summary>
    /// 垂直布局
    /// </summary>
    public class VerticalLayout : UIContainer
    {
        /// <summary>
        /// 布局的间隙
        /// </summary>
        public float Space { get; set; }

        /// <summary>
        /// 子元素高度，不保证所有子元素的宽度均为该数值
        /// </summary>
        public float ChildHeight { get; set; }

        /// <summary>
        /// 垂直布局
        /// </summary>
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;

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
            var layoutRight = Math.Max(rect.Left + Width ?? 0f, inputRect.Right);

            // 扩张容器
            foreach (var child in Children)
            {
                var childRect = new SKRect(currentPosX, currentPosY, layoutRight, currentPosY + ChildHeight);
                childRect = child.UpdateLayout(childRect);
                layoutRight = Math.Max(childRect.Right, layoutRight);
                currentPosY = Space + childRect.Height + childRect.Top;
            }

            currentPosY = rect.Top;

            // 布局
            foreach (var child in Children)
            {
                SKRect childRect;

                var childWidth = child.Width;

                if (childWidth == null && !(child.Margin.Left != null && child.Margin.Right != null))
                    childWidth = child.RenderRect.Width;

                if (childWidth != null && childWidth > 0)
                {
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Center:
                            {
                                var layoutWidth = layoutRight - currentPosX - child.Margin.Left ?? 0;
                                var layoutOffset = (layoutWidth - childWidth) / 2;

                                var layoutOffsetLeft = layoutOffset - child.Margin.Left ?? 0;
                                var layoutOffsetRight = layoutOffset - child.Margin.Right ?? 0;
                                childRect = new SKRect(currentPosX + layoutOffsetLeft, currentPosY, layoutRight - layoutOffsetRight, currentPosY + ChildHeight);
                                break;
                            }
                        case VerticalAlignment.Right:
                            {
                                var layoutWidth = layoutRight - currentPosX - child.Margin.Left ?? 0;
                                var layoutOffset = layoutWidth - childWidth;

                                var layoutOffsetLeft = layoutOffset - child.Margin.Left ?? 0;
                                var layoutOffsetRight = layoutOffset - child.Margin.Right ?? 0;
                                childRect = new SKRect(currentPosX + layoutOffsetLeft, currentPosY, layoutRight - layoutOffsetRight, currentPosY + ChildHeight);
                                break;
                            }
                        default:
                            childRect = new SKRect(currentPosX, currentPosY, layoutRight, currentPosY + ChildHeight);
                            break;
                    }
                }
                else
                {
                    childRect = new SKRect(currentPosX, currentPosY, layoutRight, currentPosY + ChildHeight);
                }
                childRect = child.UpdateLayout(childRect);
                currentPosY = Space + childRect.Height + childRect.Top;
            }

            RenderRect = new SKRect(rect.Left, rect.Top, layoutRight, currentPosY);

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
