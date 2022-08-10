using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Graphic
{
    public static class Fonts
    {
        public static SKTypeface Default { get; set; } = SKTypeface.Default;

        public static void LoadDefault(string path)
        {
            var font = SKTypeface.FromFile(path);

            if (Default != SKTypeface.Default)
                Default.Dispose();
            Default = font;
        }
    }
}
