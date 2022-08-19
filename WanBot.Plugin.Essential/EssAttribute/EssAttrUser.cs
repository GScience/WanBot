using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.Essential.EssAttribute
{
    /// <summary>
    /// 基础属性用户
    /// </summary>
    public class EssAttrUser : IAsyncDisposable
    {
        private EssAttributeDatabaseContext _db;
        private long _id;

        private DbEssAttributeUser? _userCache;

        private DbEssAttributeUser _user
        {
            get
            {
                if (_userCache != null)
                    return _userCache;

                var usr = _db.Users.SingleOrDefault(essAttrUsr => essAttrUsr.Id == _id);
                if (usr == null)
                {
                    usr = new()
                    {
                        Id = _id,
                        Money = "0",
                        Energy = 100,
                        EnergyMax = 100,
                        LastTimeCheckEnergy = DateTime.Now,
                        AttactAddition = 0,
                        MagicAddition = 0,
                        DefenceAddition = 0,
                        HpAddition = 0,
                        SpAttactAddition = 0,
                        SpDefenceAddition = 0
                    };
                    _db.Users.Add(usr);
                    _db.SaveChanges();
                }

                _userCache = usr;
                return _userCache;
            }
        }

        public BigInteger Money
        {
            get
            {
                return BigInteger.Parse(_user.Money);
            }
            set
            {
                _user.Money = value.ToString();
            }
        }

        public int EnergyMax
        {
            get => _user.EnergyMax;
            set => _user.EnergyMax = value;
        }

        /// <summary>
        /// 体力，默认每小时回复5点
        /// </summary>
        public int Energy
        {
            get
            {
                var usr = _user;
                var nowTime = DateTime.Now;
                var timeSpan = nowTime - usr.LastTimeCheckEnergy;
                usr.Energy += (int)timeSpan.TotalHours * 5;
                if (usr.Energy > usr.EnergyMax)
                    usr.Energy = usr.EnergyMax;

                // 计算新的时间
                _user.LastTimeCheckEnergy += new TimeSpan((int)timeSpan.TotalHours, 0, 0);
                return usr.Energy;
            }
            set => _user.Energy = value;
        }

        public int AttactAddition
        {
            get => _user.AttactAddition;
            set => _user.AttactAddition = value;
        }

        public int SpAttactAddition
        {
            get => _user.SpAttactAddition;
            set => _user.SpAttactAddition = value;
        }

        public int SpDefenceAddition
        {
            get => _user.SpDefenceAddition;
            set => _user.SpDefenceAddition = value;
        }

        public int HpAddition
        {
            get => _user.HpAddition;
            set => _user.HpAddition = value;
        }

        public int MagicAddition
        {
            get => _user.MagicAddition;
            set => _user.MagicAddition = value;
        }

        internal EssAttrUser(long id, EssAttributeDatabaseContext db)
        {
            _id = id;
            _db = db;
        }

        public async ValueTask DisposeAsync()
        {
            await _db.SaveChangesAsync();
            GC.SuppressFinalize(this);
        }
    }
}
