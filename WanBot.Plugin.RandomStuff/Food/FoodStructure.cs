using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.RandomStuff.Food
{
    internal class FoodStructure : IFoodGenerator
    {
        public static List<Func<string>> FoodStructureList = new()
        {
            // 油闷 大虾
            () => $"{new CookMethod().GenFood()}{new Ingredients(true, false, false).GenFood()}",
            () => $"{new CookMethod().GenFood()}{new Ingredients(false, true, false).GenFood()}",
            () => $"{new CookMethod().GenFood()}{new Ingredients(false, false, true).GenFood()}",
            () => $"{new CookMethod().GenFood()}{new Ingredients(true, true, false).GenFood()}",
            () => $"{new CookMethod().GenFood()}{new Ingredients(true, false, true).GenFood()}",
            () => $"{new CookMethod().GenFood()}{new Ingredients(false, true, true).GenFood()}",
            () => $"{new CookMethod().GenFood()}{new Ingredients(true, true, true).GenFood()}",

            // 西红柿 炒 鸡蛋
            () => $"{new Ingredients(false, false, true).GenFood()}{new CookMethod().GenFood()}{new Ingredients(true, false, false).GenFood()}",
            () => $"{new Ingredients(false, false, true).GenFood()}{new Ingredients(false, false, true).GenFood()}{new CookMethod().GenFood()}{new Ingredients(true, false, false).GenFood()}",

            // 金枪鱼 刺身
            () => $"{new Ingredients(false, true, false).GenFood()}刺身"
        };

        public string GenFood()
        {
            return FoodStructureList[new Random().Next(0, FoodStructureList.Count)]();
        }
    }
}
