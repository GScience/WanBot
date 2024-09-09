using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api
{
    public interface IPlugin
    {
        void PreInit(IWanBot bot);
        void Init(IWanBot bot);
        void PostInit(IWanBot bot);
        void OnStop(IWanBot bot);
    }
}
