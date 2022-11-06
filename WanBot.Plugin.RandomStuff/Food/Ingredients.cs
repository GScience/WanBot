using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.RandomStuff.Food
{
    internal class FishIngredients : IFoodGenerator
    {
        public static string[] IngredientList = new[]
        {
            "鳕鱼", "金枪鱼", "三文鱼", "鲟鱼", "草鱼",
            "金龙鱼", "海胆", "螃蟹", "甜虾", "猫眼螺",
            "蛏子", "扇贝", "带鱼", "鳄鱼", "鲨鱼",
            "鱼头"
        };

        public string GenFood()
        {
            return IngredientList[new Random().Next(0, IngredientList.Length)];
        }
    }

    internal class MeatIngredients : IFoodGenerator
    {
        public static string[] IngredientList = new[]
        {
            "大虾", "虾仁", "鸡丁", "鸡蛋", "里脊",
            "羊腿", "牛排", "腰子", "羊排", "鸡胸",
            "血肠", "鸭肠", "牛柳", "猪脚"
        };

        public string GenFood()
        {
            return IngredientList[new Random().Next(0, IngredientList.Length)];
        }
    }

    internal class VegetableIngredients : IFoodGenerator
    {
        public static string[] IngredientList = new[]
        {
            "韭菜", "芹菜", "香菜", "芥兰", "金针菇",
            "杏鲍菇", "白菜", "菠菜", "娃娃菜", "洋葱",
            "尖椒", "土豆丝", "苦瓜", "马铃薯"
        };

        public string GenFood()
        {
            return IngredientList[new Random().Next(0, IngredientList.Length)];
        }
    }

    internal class Ingredients : IFoodGenerator
    {
        private List<IFoodGenerator> _generateList = new();

        public Ingredients(bool allowMeet, bool allowFish, bool allowVegetable)
        {
            if (allowMeet)
                _generateList.Add(new MeatIngredients());
            if (allowFish)
                _generateList.Add(new FishIngredients());
            if (allowVegetable)
                _generateList.Add(new VegetableIngredients());
        }

        public string GenFood()
        {
            var generator = _generateList[new Random().Next(0, _generateList.Count)];
            return generator.GenFood();
        }
    }
}
