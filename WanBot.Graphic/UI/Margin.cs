using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Graphic.UI
{
    public struct Margin
    {
        public float? Left { get; set; }
        public float? Top { get; set; }
        public float? Right { get; set; }
        public float? Bottom { get; set; }

        public Margin(float left, float top)
        {
            Left = left;
            Right = null;
            Top = top;
            Bottom = null;
        }

        public Margin(float? left, float? top, float? right, float? bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
