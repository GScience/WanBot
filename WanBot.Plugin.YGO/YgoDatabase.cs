using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Plugin.YGO.CardEnum;

namespace WanBot.Plugin.YGO
{
    /// <summary>
    /// 游戏王数据库，其中的数据库加载代码、高级检索功能参考自<![CDATA[https://code.mycard.moe/mycard/YGOProUnity_V2/-/blob/master/Assets/YGOSharp/CardsManager.cs]]>
    /// </summary>
    public class YgoDatabase
    {
        public const int NotUseParam = -233;

        public int DownloadBufferSize = 8192;
        private const string DatabaseUrl = "https://code.mycard.moe/mycard/ygopro-database/-/raw/master/locales/zh-CN/cards.cdb";
        private ILogger _logger;
        private IDictionary<int, YgoCard> _cards = new Dictionary<int, YgoCard>();

        internal YgoDatabase(ILogger logger)
        {
            _logger = logger;
        }

        public async Task UpdateAsync(string databasePath)
        {
            _logger.Warn($"Update database");
            await UpdateDatabaseAsync(databasePath);

            using SqliteConnection connection = new SqliteConnection("Data Source=" + databasePath);
            connection.Open();

            using var command =
                new SqliteCommand("SELECT datas.*, texts.* FROM datas,texts WHERE datas.id=texts.id;", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
                LoadCard(reader);
        }

        /// <summary>
        /// 加载数据库
        /// </summary>
        /// <param name="databasePath"></param>
        public async Task LoadAsync(string databasePath)
        {
            if (!File.Exists(databasePath))
            {
                _logger.Warn($"Database not found");
                await UpdateDatabaseAsync(databasePath);
            }

            using var connection = new SqliteConnection("Data Source=" + databasePath);
            await connection.OpenAsync();

            using var command =
                new SqliteCommand("SELECT datas.*, texts.* FROM datas,texts WHERE datas.id=texts.id;", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
                LoadCard(reader);
            SqliteConnection.ClearPool(connection);
            await connection.CloseAsync();
        }

        /// <summary>
        /// 加载卡片
        /// </summary>
        /// <param name="reader"></param>
        private void LoadCard(IDataRecord reader)
        {
            YgoCard card = new(reader);
            if (!_cards.ContainsKey(card.Id))
                _cards.Add(card.Id, card);
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="databasePath"></param>
        private async Task UpdateDatabaseAsync(string databasePath)
        {
            var url = DatabaseUrl;
            _logger.Info($"Download database from {url}");

            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            using var ns = await response.Content.ReadAsStreamAsync();
            using var fs = File.Create(databasePath);
            var buffer = new byte[DownloadBufferSize];

            long resultLength = ns.Length;
            long totalReadLength = 0;
            int length;

            while ((length = await ns.ReadAsync(buffer)) != 0)
            {
                totalReadLength += length;
                fs.Write(buffer, 0, length);

                // 显示日志
                if (resultLength == 0)
                {
                    _logger.Info($"Downloaded: {totalReadLength / 1024.0f} KB");
                }
                else
                {
                    var maxBarValue = 100;
                    var currentBarValue = maxBarValue * (totalReadLength / (float)resultLength);

                    if (currentBarValue % 10 == 0)
                        _logger.Info($"Downloaded：{currentBarValue}%");
                }
            }
        }

        /// <summary>
        /// 高级检索
        /// </summary>
        /// <param name="getName"></param>
        /// <param name="getLevel"></param>
        /// <param name="getAttack"></param>
        /// <param name="getDefence"></param>
        /// <param name="getP"></param>
        /// <param name="getYear"></param>
        /// <param name="getLevel_UP"></param>
        /// <param name="getAttack_UP"></param>
        /// <param name="getDefence_UP"></param>
        /// <param name="getP_UP"></param>
        /// <param name="getYear_UP"></param>
        /// <param name="getOT"></param>
        /// <param name="getPack"></param>
        /// <param name="getTypeFilter"></param>
        /// <param name="getTypeFilter2"></param>
        /// <param name="getRaceFilter"></param>
        /// <param name="getAttributeFilter"></param>
        /// <param name="getCatagoryFilter"></param>
        /// <returns></returns>
        internal List<YgoCard> SearchAdvanced(
            string getName,
            int getLevel,
            int getAttack,
            int getDefence,
            int getP,
            int getLevel_UP,
            int getAttack_UP,
            int getDefence_UP,
            int getP_UP,
            int getOT,
            uint getTypeFilter,
            uint getTypeFilter2,
            uint getRaceFilter,
            uint getAttributeFilter,
            uint getCatagoryFilter
        )
        {
            List<YgoCard> returnValue = new();
            foreach (var item in _cards)
            {
                YgoCard card = item.Value;
                if ((card.Type & (uint)CardType.Token) == 0)
                {
                    if (getName == ""
                        || Regex.Replace(card.Name, getName, "miaowu", RegexOptions.IgnoreCase) != card.Name
                        || Regex.Replace(card.Desc, getName, "miaowu", RegexOptions.IgnoreCase) != card.Desc
                        || card.Id.ToString() == getName
                    )
                    {
                        if (((card.Type & getTypeFilter) == getTypeFilter || getTypeFilter == 0)
                            && ((card.Type == getTypeFilter2
                                 || getTypeFilter == (uint)CardType.Monster) &&
                                (card.Type & getTypeFilter2) == getTypeFilter2
                                || getTypeFilter2 == 0))
                        {
                            if (!(getRaceFilter == 0 || (card.Race & getRaceFilter) > 0)) continue;
                            if (!(getAttributeFilter == 0 || (card.Attribute & getAttributeFilter) > 0)) continue;
                            if (!(getCatagoryFilter == 0 || (card.Category & getCatagoryFilter) == getCatagoryFilter)) continue;
                            if (!JudgeInt(getAttack, getAttack_UP, card.Attack)) continue;
                            if (!JudgeInt(getDefence, getDefence_UP, card.Defense)) continue;
                            if (!JudgeInt(getLevel, getLevel_UP, card.Level)) continue;
                            if (!JudgeInt(getP, getP_UP, card.LScale)) continue;
                            if (!(getOT == NotUseParam || (getOT & card.Ot) == getOT)) continue;

                            returnValue.Add(card);
                        }
                    }
                }
            }
            returnValue.Sort(ComparisonOfCard(getName));
            return returnValue;
        }

        static bool JudgeInt(int min, int max, int raw)
        {
            bool result = true;
            if (min == NotUseParam && max == NotUseParam)
            {
                result = true;
            }

            if (min == NotUseParam && max != NotUseParam)
            {
                result = max == raw;
            }

            if (min != NotUseParam && max == NotUseParam)
            {
                result = min == raw;
            }

            if (min != NotUseParam && max != NotUseParam)
            {
                result = min <= raw && raw <= max;
            }

            return result;
        }

        private enum SearchMachineStage
        {
            UNKNOWN,
            Keyword,
            Kind,
            Type,
            Ot,
            Level,
            Scale,
            Attack,
            Defence,
            Attribute,
            Race,
            Other
        }

        public List<YgoCard> SearchByFilter(YgoFilter filter)
        {
            return SearchAdvanced(
               filter.Keyword,
               filter.Level,
               NotUseParam,
               NotUseParam,
               filter.Level,
               NotUseParam,
               NotUseParam,
               NotUseParam,
               NotUseParam,
               NotUseParam,
               (uint)filter.Kind,
               (uint)filter.Type,
               (uint)filter.Rare,
               (uint)filter.Attribute,
               0);
        }

        public List<YgoCard> SearchByKeyword(string getName)
        {
            return SearchAdvanced(
               getName,
               NotUseParam,
               NotUseParam,
               NotUseParam,
               NotUseParam,
               NotUseParam,
               NotUseParam,
               NotUseParam,
               NotUseParam,
               NotUseParam,
               0,
               0,
               0,
               0,
               0);
        }

        /// <summary>
        /// 检索代码
        /// kw(keyword)=关键词
        /// kd(kind)=类型，支持怪兽，魔法，陷阱
        /// tp(type)=类型，支持XYZ等
        /// ot=限制
        /// lv(level)=星\阶级，如1-4，也可以输入数值
        /// sc(scale)=刻度，如1-4，也可以输入数值
        /// atk(attack)=攻击，如1500-2000，也可以输入数值
        /// def(defence)=防御，如1500-2000，也可以输入数值
        /// att(attribute)=属性，如光地黑水炎神风，支持多个汉字组合
        /// ra(race)=种族，如恶魔，多属性需要使用分割符分离
        /// </summary>
        /// <param name="searchCode"></param>
        public List<YgoCard> SearchByCode(string searchCode)
        {
            string getName = "";
            var getLevel = NotUseParam;
            var getAttack = NotUseParam;
            var getDefence = NotUseParam;
            var getP = NotUseParam;
            var getLevel_UP = NotUseParam;
            var getAttack_UP = NotUseParam;
            var getDefence_UP = NotUseParam;
            var getP_UP = NotUseParam;
            var getOT = NotUseParam;
            uint getTypeFilter = 0;
            uint getTypeFilter2 = 0;
            uint getRaceFilter = 0;
            uint getAttributeFilter = 0;
            uint getCatagoryFilter = 0;

            // 分割字符，逗号分号空格都行
            var codeReader = searchCode.Split(',', ';', ' ').GetEnumerator();

            var currentState = SearchMachineStage.UNKNOWN;

            // 解析搜索关键词
            while (codeReader.MoveNext())
            {
                // 检查当前输入是否为关键词，如果是的话先进入关键词处理设置当前状态
                var current = (string)codeReader.Current;
                switch (current.ToLower())
                {
                    // 关键词
                    case "kw":
                    case "keyword":
                        currentState = SearchMachineStage.Keyword;
                        continue;

                    // 类型，支持关键词怪兽，魔法，陷阱
                    case "kd":
                    case "kind":
                        currentState = SearchMachineStage.Kind;
                        continue;

                    // 类型，支持XYZ等
                    case "tp":
                    case "type":
                        currentState = SearchMachineStage.Type;
                        continue;

                    // 限制，支持ocg,tcg
                    case "ot":
                        currentState = SearchMachineStage.Ot;
                        continue;

                    // 星\阶级，如1-4，也可以输入数值
                    case "lv":
                    case "level":
                        currentState = SearchMachineStage.Level;
                        continue;

                    // 刻度，如1-4，也可以输入数值
                    case "sc":
                    case "scale":
                        currentState = SearchMachineStage.Scale;
                        continue;

                    // 攻击，如1500-2000，也可以输入数值
                    case "atk":
                    case "attack":
                        currentState = SearchMachineStage.Attack;
                        continue;

                    // 防御，如1500-2000，也可以输入数值
                    case "def":
                    case "defence":
                        currentState = SearchMachineStage.Defence;
                        continue;

                    // 属性，如光地黑水炎神风，支持多个汉字组合
                    case "att":
                    case "attribute":
                        currentState = SearchMachineStage.Attribute;
                        continue;

                    // 种族，如恶魔，多属性需要使用分割符分离
                    case "ra":
                    case "race":
                        currentState = SearchMachineStage.Race;
                        continue;

                    // 其他，如XYZ，多属性需要使用分割符分离
                    case "other":
                        currentState = SearchMachineStage.Other;
                        continue;
                }

                // 不是的话根据当前状态读取数据
                switch (currentState)
                {
                    // 关键词
                    case SearchMachineStage.Keyword:
                        if (!string.IsNullOrEmpty(getName))
                            throw new ArgumentException(ConstructureErrorMessage(codeReader, "重复指定关键词"));
                        getName = current;
                        break;

                    // 类型，支持关键词怪兽，魔法，陷阱
                    case SearchMachineStage.Kind:
                        if (getTypeFilter != 0)
                            throw new ArgumentException(ConstructureErrorMessage(codeReader, "重复指定类型"));
                        getTypeFilter = current switch
                        {
                            "怪兽" => (uint)CardType.Monster,
                            "魔法" => (uint)CardType.Spell,
                            "陷阱" => (uint)CardType.Trap,
                            _ => throw new ArgumentException(ConstructureErrorMessage(codeReader, $"卡片种类仅支持怪兽、魔法以及陷阱，不支持{current}"))
                        };
                        break;

                    // 类型，支持XYZ等
                    case SearchMachineStage.Type:
                        getTypeFilter2 |= current.ToLower() switch
                        {
                            "通常" => (uint)CardType.Normal,
                            "效果" => (uint)CardType.Effect,
                            "融合" => (uint)CardType.Fusion,
                            "仪式" => (uint)CardType.Ritual,
                            "陷阱怪兽" => (uint)CardType.TrapMonster,
                            "灵魂" => (uint)CardType.Spirit,
                            "同盟" => (uint)CardType.Union,
                            "二重" => (uint)CardType.Dual,
                            "调整" => (uint)CardType.Tuner,
                            "同调" => (uint)CardType.Synchro,
                            "衍生物" => (uint)CardType.Token,
                            "速攻" => (uint)CardType.QuickPlay,
                            "永续" => (uint)CardType.Continuous,
                            "装备" => (uint)CardType.Equip,
                            "场地" => (uint)CardType.Field,
                            "反击" => (uint)CardType.Counter,
                            "反转" => (uint)CardType.Flip,
                            "卡通" => (uint)CardType.Toon,
                            "超量" => (uint)CardType.Xyz,
                            "xyz" => (uint)CardType.Xyz,
                            "灵摆" => (uint)CardType.Pendulum,
                            "特殊召唤" => (uint)CardType.SpSummon,
                            "特召" => (uint)CardType.SpSummon,
                            "连接" => (uint)CardType.Link,
                            "链接" => (uint)CardType.Link,
                            _ => throw new ArgumentException(ConstructureErrorMessage(codeReader, $"卡片类型仅支持XYZ，通常等，不支持{current}"))
                        };
                        break;

                    // 限制，支持ocg,tcg
                    case SearchMachineStage.Ot:
                        if (getOT != NotUseParam)
                            throw new ArgumentException(ConstructureErrorMessage(codeReader, "重复指定OT"));
                        getOT = current.ToLower() switch
                        {
                            "ocg" => (int)CardLimitOt.OCG,
                            "tcg" => (int)CardLimitOt.TCG,
                            "简中" => (int)CardLimitOt.SimpleChinese,
                            _ => throw new ArgumentException(ConstructureErrorMessage(codeReader, "OT仅支持ocg、tcg和简中"))
                        };
                        break;

                    // 星\阶级，如1-4，也可以输入数值
                    case SearchMachineStage.Level:
                        if (getLevel != NotUseParam)
                            throw new ArgumentException(ConstructureErrorMessage(codeReader, "重复指定星/阶级"));
                        try
                        {
                            var (min, max) = GetRangeFromString(current);
                            getLevel = min;
                            getLevel_UP = max;
                        }
                        catch (Exception e)
                        {
                            throw new ArgumentException(ConstructureErrorMessage(codeReader, e.Message));
                        }
                        break;

                    // 刻度，如1-4，也可以输入数值
                    case SearchMachineStage.Scale:
                        if (getP != NotUseParam)
                            throw new ArgumentException(ConstructureErrorMessage(codeReader, "重复指定刻度"));
                        try
                        {
                            var (min, max) = GetRangeFromString(current);
                            getP = min;
                            getP_UP = max;
                        }
                        catch (Exception e)
                        {
                            throw new ArgumentException(ConstructureErrorMessage(codeReader, e.Message));
                        }
                        break;

                    // 攻击，如1500-2000，也可以输入数值
                    case SearchMachineStage.Attack:
                        if (getP != NotUseParam)
                            throw new ArgumentException(ConstructureErrorMessage(codeReader, "重复指定攻击"));
                        try
                        {
                            var (min, max) = GetRangeFromString(current);
                            getAttack = min;
                            getAttack_UP = max;
                        }
                        catch (Exception e)
                        {
                            throw new ArgumentException(ConstructureErrorMessage(codeReader, e.Message));
                        }
                        break;

                    // 防御，如1500-2000，也可以输入数值
                    case SearchMachineStage.Defence:
                        if (getP != NotUseParam)
                            throw new ArgumentException(ConstructureErrorMessage(codeReader, "重复指定防御"));
                        try
                        {
                            var (min, max) = GetRangeFromString(current);
                            getDefence = min;
                            getDefence_UP = max;
                        }
                        catch (Exception e)
                        {
                            throw new ArgumentException(ConstructureErrorMessage(codeReader, e.Message));
                        }
                        break;

                    // 属性，如光地黑水炎神风，支持多个汉字组合
                    case SearchMachineStage.Attribute:
                        foreach (var c in current)
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
                                _ => throw new ArgumentException(ConstructureErrorMessage(codeReader, "未知的属性：" + c))
                            };

                            getAttributeFilter |= (uint)attr;
                        }
                        break;

                    // 种族，如恶魔，多属性需要使用分割符分离
                    case SearchMachineStage.Race:
                        var rare = current switch
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
                            _ => throw new ArgumentException(ConstructureErrorMessage(codeReader, "未知的种族：" + current))
                        };
                        getRaceFilter |= (uint)rare;
                        break;
                    default:
                        throw new ArgumentException(ConstructureErrorMessage(codeReader, "查询代码错误，未指定任何查询代码" + current));
                }
            }

            return SearchAdvanced(
                getName,
                getLevel,
                getAttack,
                getDefence,
                getP,
                getLevel_UP,
                getAttack_UP,
                getDefence_UP,
                getP_UP,
                getOT,
                getTypeFilter,
                getTypeFilter2,
                getRaceFilter,
                getAttributeFilter,
                getCatagoryFilter);
        }

        /// <summary>
        /// 从字符串创建范围
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static (int min, int max) GetRangeFromString(string str)
        {
            (int min, int max) result;

            var range = str.Split('-');
            switch (range.Length)
            {
                case 0:
                    throw new ArgumentException("范围缺少参数，应为 数字-数字");
                case 1:
                    if (!int.TryParse(range[0], out result.max))
                        throw new ArgumentException("范围内存在非法内容，应为 数字-数字");
                    result.min = result.max;
                    break;
                case 2:
                    if (!int.TryParse(range[0], out result.min) || !int.TryParse(range[1], out result.max))
                        throw new ArgumentException("范围内存在非法内容，应为 数字-数字");
                    break;
                default:
                    throw new ArgumentException("范围参数过多，应为 数字-数字");
            }

            // 大小不对则交换
            if (result.min > result.max)
            {
                var max = result.min;
                result.min = result.max;
                result.max = max;
            }

            return result;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <returns></returns>
        internal static Comparison<YgoCard> ComparisonOfCard(string nameInSearch)
        {
            return (left, right) =>
            {
                if (left.Name == nameInSearch && right.Name != nameInSearch)
                    return -1;
                else if (right.Name == nameInSearch && left.Name != nameInSearch)
                    return 1;

                if ((left.Type & 7) < (right.Type & 7)) return -1;
                else if ((left.Type & 7) > (right.Type & 7)) return 1;

                if ((left.Type >> 3) > (right.Type >> 3)) return 1;
                else if ((left.Type >> 3) < (right.Type >> 3)) return -1;

                if (left.Level > right.Level) return -1;
                else if (left.Level < right.Level) return 1;

                if (left.Attack > right.Attack) return -1;
                else if (left.Attack < right.Attack) return 1;

                if (left.Attribute > right.Attribute) return 1;
                else if (left.Attribute < right.Attribute) return -1;

                if (left.Race > right.Race) return 1;
                else if (left.Race < right.Race) return -1;

                if (left.Category > right.Category) return 1;
                else if (left.Category < right.Category) return -1;

                if (left.Id > right.Id) return 1;
                else if (left.Id < right.Id) return -1;

                return 0;
            };
        }

        private static string ConstructureErrorMessage(IEnumerator reader, string message)
        {
            return $"检索代码中存在错误：{message}。错误发生位置=>{(string)reader.Current}";
        }
    }
}
