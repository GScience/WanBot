﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.RandomStuff.Food
{
    /// <summary>
    /// 随机食物
    /// </summary>
    public class RandomFoodGenerator : IFoodGenerator
    {
        public static string[] FoodList = new[]
        {
            "面包", "蛋糕", "荷包蛋", "烧饼", "饽饽", 
            "肉夹馍", "油条", "馄饨", "火腿", "面条", 
            "小笼包", "玉米粥", "肉包", "煎饼果子", "饺子", 
            "煎蛋", "烧卖", "生煎", "锅贴", "包子", 
            "酸奶", "苹果", "梨", "香蕉", "皮蛋瘦肉粥", 
            "蛋挞", "南瓜粥", "煎饼", "玉米糊", "泡面", 
            "粥", "馒头", "燕麦片", "水煮蛋", "米粉",
            "豆浆", "牛奶", "花卷", "豆腐脑", "煎饼果子", 
            "小米粥", "黑米糕", "鸡蛋饼", "牛奶布丁", "水果沙拉", 
            "鸡蛋羹", "南瓜馅饼", "鸡蛋灌饼", "奶香小馒头", "汉堡包", 
            "披萨", "八宝粥", "三明治", "蛋包饭", "豆沙红薯饼", 
            "驴肉火烧", "粥", "粢饭糕", "蒸饺", "白粥", 
            "盖浇饭", "砂锅", "大排档", "米线", "满汉全席", 
            "西餐", "麻辣烫", "自助餐", "炒面", "快餐", 
            "水果", "西北风", "馄饨", "火锅", "烧烤", 
            "泡面", "水饺", "日本料理", "涮羊肉", "味千拉面", 
            "面包", "扬州炒饭", "自助餐", "菜饭骨头汤", "茶餐厅", 
            "海底捞", "西贝莜面村", "披萨", "麦当劳", "KFC", 
            "汉堡王", "卡乐星", "兰州拉面", "沙县小吃", "烤鱼",
            "烤肉", "海鲜", "铁板烧", "韩国料理", "粥", 
            "快餐", "萨莉亚", "桂林米粉", "东南亚菜", "甜点",
            "农家菜", "川菜", "粤菜", "湘菜", "本帮菜", 
            "生活", "全家便当"
        };

        public string GenFood()
        {
            var rand = new Random();

            if (rand.Next(0, 3) != 1)
            {
                // 从数据库里找食物
                return FoodList[rand.Next(0, FoodList.Length)];
            }
            else
                return new FoodStructure().GenFood();
        }
    }
}
