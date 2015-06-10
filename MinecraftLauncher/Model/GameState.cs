using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftLauncher.Model
{
    [Serializable]
    public enum GameState
    {
        [Description("Установить")]
        NotInstalled,
        [Description("Отменить")]
        Downloading,
        [Description("Играть")]
        Installed,
        [Description("Отменить")]
        Checking,
        [Description("Закрыть")]
        Playing
    }
}
