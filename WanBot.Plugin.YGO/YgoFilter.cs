using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Plugin.YGO.CardEnum;

namespace WanBot.Plugin.YGO
{
    public class YgoFilter
    {
        public static string[] Separator =
        {
            "属性",
            "族",
            "级",
            "星",
            "阶"
        };

        public static string[] RareWord =
        {
            "战士",
            "魔法师",
            "天使",
            "恶魔",
            "不死",
            "机械",
            "水",
            "炎",
            "岩石",
            "鸟兽",
            "植物",
            "昆虫",
            "雷",
            "龙",
            "兽",
            "兽战士",
            "恐龙",
            "鱼",
            "海龙",
            "爬虫",
            "念动力",
            "幻神兽",
            "创造神",
            "幻龙",
            "电子界"
        };

        public static string[] TypeWord =
        {
            "通常",
            "效果",
            "融合",
            "仪式",
            "陷阱怪兽",
            "灵魂",
            "同盟",
            "二重",
            "调整",
            "同调",
            "衍生物",
            "速攻",
            "永续",
            "装备",
            "场地",
            "反击",
            "反转",
            "卡通",
            "超量",
            "xyz",
            "灵摆",
            "特殊召唤",
            "特召",
            "连接",
            "链接"
        };

        public static string[] KindWord =
        {
            "怪兽",
            "魔法",
            "陷阱"
        };

        public static string[] AttributeWord =
        {
            "地",
            "水",
            "炎",
            "风",
            "光",
            "黑",
            "神"
        };

        internal static IEnumerable<string> YgoWordsEnumerator()
        {
            foreach (var word in RareWord) yield return word;
            foreach (var word in TypeWord) yield return word;
            foreach (var word in KindWord) yield return word;
            foreach (var word in AttributeWord) yield return word;
            foreach (var word in Separator) yield return word;
        }

        public string Keyword { get; private set; } = string.Empty;
        public int Level { get; private set; } = YgoDatabase.NotUseParam;
        public CardType Kind { get; private set; }
        public CardType Type { get; private set; }
        public CardAttribute Attribute { get; private set; }
        public CardRace Rare { get; private set; }

        public void SetLevel(int level)
        {
            Level = level;
        }

        public void SetKeyword(string keyword)
        {
            Keyword = keyword;
        }

        public void SetRace(string race)
        {
            Rare = race switch
            {
                "战士" => CardRace.Warrior,
                "魔法师" => CardRace.SpellCaster,
                "天使" => CardRace.Fairy,
                "恶魔" => CardRace.Fiend,
                "不死" => CardRace.Zombie,
                "机械" => CardRace.Machine,
                "水" => CardRace.Aqua,
                "炎" => CardRace.Pyro,
                "岩石" => CardRace.Rock,
                "鸟兽" => CardRace.WindBeast,
                "植物" => CardRace.Plant,
                "昆虫" => CardRace.Insect,
                "雷" => CardRace.Thunder,
                "龙" => CardRace.Dragon,
                "兽" => CardRace.Beast,
                "兽战士" => CardRace.BestWarrior,
                "恐龙" => CardRace.Dinosaur,
                "鱼" => CardRace.Fish,
                "海龙" => CardRace.SeaSerpent,
                "爬虫" => CardRace.Reptile,
                "念动力" => CardRace.Psycho,
                "幻神兽" => CardRace.DivineBeast,
                "创造神" => CardRace.CreatorGod,
                "幻龙" => CardRace.Wyrm,
                "电子界" => CardRace.Cyberse,
                _ => CardRace.Unknown
            };
        }

        public void SetAttribute(string attributes)
        {
            Attribute = 0;
            foreach (var c in attributes)
            {
                var attr = c switch
                {
                    '地' => CardAttribute.Earth,
                    '水' => CardAttribute.Water,
                    '炎' => CardAttribute.Fire,
                    '风' => CardAttribute.Wind,
                    '光' => CardAttribute.Light,
                    '黑' => CardAttribute.Dark,
                    '神' => CardAttribute.Divine,
                    _ => CardAttribute.Unknown
                };

                Attribute |= attr;
            }
        }

        public void SetType(IEnumerable<string> types)
        {
            Type = 0;
            foreach (var type in types)
            {
                var cardType = type.ToLower() switch
                {
                    "通常" => CardType.Normal,
                    "效果" => CardType.Effect,
                    "融合" => CardType.Fusion,
                    "仪式" => CardType.Ritual,
                    "陷阱怪兽" => CardType.TrapMonster,
                    "灵魂" => CardType.Spirit,
                    "同盟" => CardType.Union,
                    "二重" => CardType.Dual,
                    "调整" => CardType.Tuner,
                    "同调" => CardType.Synchro,
                    "衍生物" => CardType.Token,
                    "速攻" => CardType.QuickPlay,
                    "永续" => CardType.Continuous,
                    "装备" => CardType.Equip,
                    "场地" => CardType.Field,
                    "反击" => CardType.Counter,
                    "反转" => CardType.Flip,
                    "卡通" => CardType.Toon,
                    "超量" => CardType.Xyz,
                    "xyz" => CardType.Xyz,
                    "灵摆" => CardType.Pendulum,
                    "特殊召唤" => CardType.SpSummon,
                    "特召" => CardType.SpSummon,
                    "连接" => CardType.Link,
                    "链接" => CardType.Link,
                    _ => CardType.Unknown
                };

                Type |= cardType;
            }
        }

        public void SetKind(string kind)
        {
            Kind = kind switch
            {
                "怪兽" => CardType.Monster,
                "魔法" => CardType.Spell,
                "陷阱" => CardType.Trap,
                _ => CardType.Unknown
            };
        }

        private YgoFilter()
        {
        }

        public static YgoFilter FromString(string filterCode)
        {
            var filter = new YgoFilter();

            // 分词算法
            var lastWord = string.Empty;
            var cardType = new List<string>();

            for (var i = 0; i < filterCode.Length; )
            {
                // 寻找当前词语
                string? currentWord = null;
                foreach (var word in YgoWordsEnumerator())
                {
                    if (word.Length + i > filterCode.Length)
                        continue;

                    if (filterCode.IndexOf(word, i, word.Length) == -1)
                        continue;
                    currentWord = word;
                    break;
                }

                // 如果无法进行分词就直接当作关键词
                if (string.IsNullOrEmpty(currentWord))
                {
                    filter.Keyword = filterCode[i..];
                    break;
                }
                else
                    i += currentWord.Length;

                // 判断是否为分割符，如果是则根据分隔符来配置上一个参数
                switch (currentWord)
                {
                    case "属性":
                        filter.SetAttribute(lastWord);
                        lastWord = string.Empty;
                        break;
                    case "族":
                        filter.SetRace(lastWord);
                        lastWord = string.Empty;
                        break;
                    case "级":
                    case "星":
                    case "阶":
                        if (int.TryParse(lastWord, out var level))
                            filter.SetLevel(level);
                        lastWord = string.Empty;
                        break;
                    default:
                        {
                            // 是否为卡片类型
                            if (TypeWord.Contains(currentWord))
                            {
                                cardType.Add(currentWord);
                                lastWord = string.Empty;
                            }
                            else if (KindWord.Contains(currentWord))
                            {
                                filter.SetKind(currentWord);
                                lastWord = string.Empty;
                            }
                            else
                                lastWord = currentWord;
                            break;
                        }
                }
            }
            filter.SetType(cardType);
            return filter;
        }
    }
}
