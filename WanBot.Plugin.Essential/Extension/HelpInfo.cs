using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Graphic.UI;
using WanBot.Graphic.UI.Layout;
using WanBot.Graphic.Util;

namespace WanBot.Plugin.Essential.Extension
{
    /// <summary>
    /// 命令帮助
    /// </summary>
    public struct HelpInfo : IHelp
    {
        public string info;

        public HorizontalAlignment alignment;

        public UIElement ToUI(float width)
        {
            var vertical = new VerticalHelper();

            vertical
                .Box(info, SKColors.White, SKColors.Black, 14, textAlignment: SKTextAlign.Right)
                .Width(width);

            return vertical.VerticalLayout;
        }
    }
}
