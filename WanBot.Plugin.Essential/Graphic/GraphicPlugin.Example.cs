using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Graphic.UI;
using WanBot.Graphic.UI.Layout;

namespace WanBot.Plugin.Essential.Graphic
{
    public partial class GraphicPlugin
    {
        public Grid GetExample(string title, SKImage? icon)
        {
            var grid = new Grid();
            grid.Width = 500;
            grid.Height = 500;

            // 背景
            var gridBg = new Rectangle();
            gridBg.Radius = new SKSize(50, 50);
            gridBg.Paint.Color = SKColors.DarkGray;
            grid.Children.Add(gridBg);

            // 垂直布局
            var verticalLayout = new VerticalLayout();
            verticalLayout.HorizontalAlignment = HorizontalAlignment.Center;

            {
                //文本框
                var textBox = new TextBox();
                textBox.Text = title;
                textBox.Margin = new Margin(50, 50, 50, 0);
                textBox.FontPaint.Color = SKColors.DarkBlue;

                verticalLayout.Children.Add(textBox);

                // 头像
                var imageBox = new ImageBox();
                imageBox.Image = icon;
                imageBox.Width = 300;
                imageBox.Height = 300;
                verticalLayout.Children.Add(imageBox);
            }
            grid.Children.Add(verticalLayout);

            return grid;
        }
    }
}
