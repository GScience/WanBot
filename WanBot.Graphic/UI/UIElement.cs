using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Graphic.UI
{
    /// <summary>
    /// 所有UI元素的基类
    /// </summary>
    public abstract class UIElement : IDisposable
    {
        public SKPaint? DebugPaint { get; set; }

        /// <summary>
        /// UI元素名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 距离父对象边界的距离，其中的null项会自动计算
        /// </summary>
        public Margin Margin { get; set; } = new Margin(0, 0, 0, 0);

        /// <summary>
        /// 请求的宽度，null则自动计算
        /// </summary>
        public float? Width { get; set; }

        /// <summary>
        /// 请求的高度，null则自动计算
        /// </summary>
        public float? Height { get; set; }

        /// <summary>
        /// 渲染区域
        /// </summary>
        public SKRect RenderRect { get; protected set; }

        /// <summary>
        /// UI区域
        /// </summary>
        public SKRect ContentRect { get; protected set; }

        /// <summary>
        /// 渲染
        /// </summary>
        /// <param name="canvas"></param>
        public virtual void Draw(SKCanvas canvas)
        {
        }

        /// <summary>
        /// 渲染调试
        /// </summary>
        /// <param name="canvas"></param>
        public virtual void DrawDebug(SKCanvas canvas)
        {
            if (DebugPaint == null)
            {
                var rand = new Random();
                DebugPaint = new SKPaint();
                var buff = new byte[3];
                rand.NextBytes(buff);
                DebugPaint.Color = new SKColor(buff[0], buff[1], buff[2], 128);
            }
            canvas.DrawRect(RenderRect, DebugPaint);
        }

        /// <summary>
        /// 刷新布局
        /// </summary>
        public virtual SKRect UpdateLayout(SKRect rect)
        {
            if (Margin.Left != null)
                rect.Left += Margin.Left.Value;
            if (Margin.Top != null)
                rect.Top += Margin.Top.Value;

            if (Margin.Right != null)
                rect.Right -= Margin.Right.Value;
            if (Margin.Bottom != null)
                rect.Bottom -= Margin.Bottom.Value;

            if (Width != null)
            {
                if (Margin.Left != null)
                    rect.Right = rect.Left + Margin.Left.Value + Width.Value;
                else if (Margin.Right != null)
                    rect.Left = rect.Right - (Margin.Right.Value - Width.Value);
                else
                    throw new Exception("Failed to calculate width");
            }

            if (Height != null)
            {
                if (Margin.Top != null)
                    rect.Bottom = rect.Top + Height.Value;
                else if (Margin.Bottom != null)
                    rect.Top = rect.Bottom - Height.Value;
                else
                    throw new Exception("Failed to calculate height");
            }

            RenderRect = rect;
            var uiRect = rect;
            if (Margin.Right != null)
                uiRect.Right += Margin.Right.Value;
            if (Margin.Bottom != null)
                uiRect.Bottom += Margin.Bottom.Value;
            ContentRect = uiRect;

            return ContentRect;
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
