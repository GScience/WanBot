using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Graphic.Template;
using WanBot.Graphic.UI;
using WanBot.Graphic.UI.Layout;

namespace WanBot.Graphic.Util
{
    /// <summary>
    /// 垂直布局助手
    /// </summary>
    public class VerticalHelper
    {
        public VerticalLayout VerticalLayout = new();
        
        public VerticalHelper Add(UIElement element)
        {
            VerticalLayout.Children.Add(element);
            return this;
        }

        public VerticalHelper Box(
            string text,
            float fontSize = 32,
            float margin = 0,
            float radius = 0,
            SKTextAlign textAlignment = SKTextAlign.Center)
        {
            return Add(new Box(text, fontSize, SKColors.Black, SKColors.LightGray, textAlignment, margin, radius, radius));
        }

        public VerticalHelper Box(
            string text,
            SKColor bgColor,
            SKColor fontColor,
            float fontSize = 32,
            float margin = 0,
            float radius = 0,
            SKTextAlign textAlignment = SKTextAlign.Center)
        {
            return Add(new Box(text, fontSize, fontColor, bgColor, textAlignment, margin, radius, radius));
        }

        public VerticalHelper Width(float width)
        {
            VerticalLayout.Width = width;
            return this;
        }

        public VerticalHelper Space(float space)
        {
            VerticalLayout.Space = space;

            return this;
        }

        public VerticalHelper Center()
        {
            VerticalLayout.VerticalAlignment = VerticalAlignment.Center;
            return this;
        }
    }
}
