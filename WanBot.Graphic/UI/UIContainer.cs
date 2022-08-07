using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Graphic.UI
{
    /// <summary>
    /// UI容器，根据内容自适应大小
    /// </summary>
    public abstract class UIContainer : UIElement
    {
        /// <summary>
        /// 子元素
        /// </summary>
        public List<UIElement> Children { get; } = new();

        public override void Draw(SKCanvas canvas)
        {
            foreach (var child in Children)
                child.Draw(canvas);
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var child in Children)
                child.Dispose();
        }
    }
}
