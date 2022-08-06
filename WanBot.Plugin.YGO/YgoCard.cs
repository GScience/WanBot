using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Plugin.YGO.CardEnum;

namespace WanBot.Plugin.YGO
{
    public class YgoCard
    {
        public int Id;
        public int Ot;
        public int Alias;
        public long Setcode;
        public int Type;

        public int Level;
        public int LScale;
        public int RScale;
        public int LinkMarker;

        public int Attribute;
        public int Race;
        public int Attack;
        public int Defense;
        public int rAttack;
        public int rDefense;

        public Int64 Category;
        public string Name;
        public string Desc;
        public string[] Str;

        public bool HasType(CardType type)
        {
            return ((Type & (int)type) != 0);
        }

        public bool HasLinkMarker(CardLinkMarker dir)
        {
            return (LinkMarker & (int)dir) != 0;
        }

        public bool IsExtraCard()
        {
            return (HasType(CardType.Fusion) || HasType(CardType.Synchro) || HasType(CardType.Xyz) || HasType(CardType.Link));
        }

        public string GetAttributeName()
        {
            return Attribute switch
            {
                (int)CardAttribute.Earth => "地",
                (int)CardAttribute.Water => "水",
                (int)CardAttribute.Fire => "炎",
                (int)CardAttribute.Wind => "风",
                (int)CardAttribute.Light => "光",
                (int)CardAttribute.Dark => "暗",
                (int)CardAttribute.Divine => "神",
                _ =>
                    HasType(CardType.Trap) ? "陷" :
                    HasType(CardType.Spell) ? "魔" : "？"
            };
        }

        public YgoCard(IDataRecord reader)
        {
            Str = new string[16];
            Id = (int)reader.GetInt64(0);
            Ot = reader.GetInt32(1);
            Alias = (int)reader.GetInt64(2);
            Setcode = reader.GetInt64(3);
            Type = (int)reader.GetInt64(4);
            Attack = reader.GetInt32(5);
            Defense = reader.GetInt32(6);
            rAttack = Attack;
            rDefense = Defense;
            long Level_raw = reader.GetInt64(7);
            Level = (int)Level_raw & 0xff;
            LScale = (int)((Level_raw >> 0x18) & 0xff);
            RScale = (int)((Level_raw >> 0x10) & 0xff);
            LinkMarker = Defense;
            Race = reader.GetInt32(8);
            Attribute = reader.GetInt32(9);
            Category = reader.GetInt64(10);
            Name = reader.GetString(12);
            Desc = reader.GetString(13);
            for (int ii = 0; ii < 0x10; ii++)
                Str[ii] = reader.GetString(14 + ii);
        }
    }
}
