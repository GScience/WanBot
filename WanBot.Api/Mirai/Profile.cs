using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai
{
    public class Profile
    {
        public string Nickname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Level { get; set; }
        public string Sign { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
    }
}
