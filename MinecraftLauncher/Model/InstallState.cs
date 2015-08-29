using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftLauncher.Model
{
    [Serializable]
    public enum InstallState
    {
        NotInstalled,
        Downloading,
        Downloaded,
        Installing,
        Installed,
        Verifying,
    }
}
