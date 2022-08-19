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
    public struct HelpCommand : IHelp
    {
        public string command;
        public string usage;

        public UIElement ToUI(float width)
        {
            var vertical = new VerticalHelper();

            vertical
                .Box(command, 18, textAlignment: SKTextAlign.Left)
                .Box(usage, 15, textAlignment: SKTextAlign.Right)
                .Space(0)
                .Width(width);

            return vertical.VerticalLayout;
        }
    }
}
