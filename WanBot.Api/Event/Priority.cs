using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Event
{
    public static class Priority
    {
        public const int BottomMost = int.MinValue;
        public const int Lowest = -10000;
        public const int Lower = -1000;
        public const int Low = -100;
        public const int Default = 0;
        public const int High = 100;
        public const int Higher = 1000;
        public const int Highest = 10000;
        public const int TopMost = int.MaxValue;
    }
}
