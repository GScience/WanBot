using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.AI
{
    internal class AIConfig
    {
        public string TongYiApiKey { get; set; } = string.Empty;
        public string TongYiModel { get; set; } = string.Empty;
        public string SystemPrompt { get; set; } = string.Empty;
    }
}
