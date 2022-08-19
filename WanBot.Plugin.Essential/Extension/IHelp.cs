using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Graphic.UI;

namespace WanBot.Plugin.Essential.Extension
{
    public interface IHelp
    {
        /// <summary>
        /// 转换为UI
        /// </summary>
        /// <returns></returns>
        public UIElement ToUI(float width);
    }
}
