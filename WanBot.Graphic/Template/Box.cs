using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Graphic.UI;
using WanBot.Graphic.UI.Layout;

namespace WanBot.Graphic.Template
{
    /// <summary>
    /// 消息盒
    /// </summary>
    public class Box : Grid
    {
        public Rectangle Background;
        public TextBox TextBox;

        public Box(string text, float fontSize, SKColor fontColor, SKColor bgColor, SKTextAlign alignment, float margin = 0, float radiusX = 15, float radiusY = 15)
        {
            Background = new();
            Background.Paint.Color = bgColor;
            Background.Radius = new SKSize(radiusX, radiusY);

            TextBox = new();
            TextBox.Text = text;
            TextBox.FontPaint.TextAlign = alignment;
            TextBox.FontPaint.TextSize = fontSize;
            TextBox.FontPaint.Color = fontColor;
            TextBox.Margin = new Margin(margin, margin, margin, margin);
            Children.Add(Background);
            Children.Add(TextBox);
        }
    }
}
