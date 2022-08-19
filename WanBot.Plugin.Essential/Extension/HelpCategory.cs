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
    public struct HelpCategory : IHelp
    {
        public string category;

        public UIElement ToUI(float width)
        {
            var vertical = new VerticalHelper();

            vertical
                .Box(category, SKColors.White, SKColors.Black, 21)
                .Width(width);

            return vertical.VerticalLayout;
        }
    }
}
