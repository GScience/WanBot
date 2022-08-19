using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Graphic;
using WanBot.Graphic.UI;
using WanBot.Graphic.UI.Layout;

namespace WanBot.Plugin.Essential.Extension
{
    /// <summary>
    /// Bot帮助
    /// </summary>
    public class BotHelp : IDisposable
    {
        /// <summary>
        /// 帮助缓存
        /// </summary>
        private SKImage? _helpCache;

        private List<IHelp> _help = new();

        public void AddHelp(IHelp help)
        {
            _help.Add(help);
        }

        public BotHelp Info(string info, HorizontalAlignment alignment = HorizontalAlignment.Left)
        {
            AddHelp(new HelpInfo { info = info, alignment = alignment });
            return this;
        }

        public BotHelp Command(string command, string usage)
        {
            AddHelp(new HelpCommand { command = command, usage = usage });
            return this;
        }

        public BotHelp Category(string category)
        {
            AddHelp(new HelpCategory { category = category });
            return this;
        }

        public void Dispose()
        {
            _helpCache?.Dispose();
        }

        public SKImage GetHelpImage(UIRenderer renderer)
        {
            if (_helpCache == null)
                _helpCache = GenHelp(renderer, 1000, 300, 3);
            return _helpCache;
        }

        private SKImage GenHelp(UIRenderer renderer, int height, int columnWidth, int column)
        {
            using var grid = new Grid();
            grid.Height = height + 30;
            grid.Margin = new Margin(0, 0);
            var bg = new Rectangle();
            bg.Margin = new Margin(0, 0, 0, 0);
            bg.Height = height + 30;
            bg.Paint.Color = SKColors.White;

            grid.Children.Add(bg);

            var horizontalLayout = new HorizontalLayout();
            horizontalLayout.Margin = new Margin(15, 15);
            horizontalLayout.Height = height;
            horizontalLayout.Space = 15;

            var helpIndex = 0;

            for (var i = 0; i < column; ++i)
            {
                var verticalLayout = new VerticalLayout();

                float heightRemain = height;

                while(helpIndex < _help.Count)
                {
                    var help = _help[helpIndex];
                    var helpUI = help.ToUI(columnWidth);
                    var helpUIRect = helpUI.UpdateLayout(SKRect.Empty);
                    heightRemain -= helpUIRect.Height;
                    if (heightRemain < 0)
                    {
                        helpUI.Dispose();
                        break;
                    }
                    ++helpIndex;

                    verticalLayout.Children.Add(helpUI);
                    heightRemain -= verticalLayout.Space;
                }
                horizontalLayout.Children.Add(verticalLayout);
            }

            grid.Children.Add(horizontalLayout);

            return renderer.Draw(grid);
        }
    }
}
