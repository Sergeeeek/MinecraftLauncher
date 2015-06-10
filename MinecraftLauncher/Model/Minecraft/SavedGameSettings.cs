using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftLauncher.Model.Minecraft
{
    [Serializable]
    class SavedGameSettings
    {
        public string GameFolder { get; set; }
        public string Arguments { get; set; }
    }
}
