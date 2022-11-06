using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.RandomStuff.Food
{
    internal class CookMethod : IFoodGenerator
    {
        private bool _multiWord;

        public static string[] CookMethodList = new[]
        {
            "油爆", "油闷", "宫保", "凉拌", "冰糖",
            "干拌", "干烧", "香辣", "清蒸", "白灼",
            "糖醋", "蒜蓉", "油泼"
        };
        
        public static string[] CookMethodSingleWordList = new[]
        {
            "炒", "煎", "烹", "炸", "蒸", "煮"
        };

        public CookMethod(bool multiWord)
        {
            _multiWord = multiWord;
        }

        public CookMethod() : this(false)
        {
        }

        public string GenFood()
        {
            if (_multiWord && new Random().Next(0, 2) == 0)
                return CookMethodList[new Random().Next(0, CookMethodList.Length)];
            return CookMethodSingleWordList[new Random().Next(0, CookMethodSingleWordList.Length)];
        }
    }
}
