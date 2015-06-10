using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftLauncher.Services.Interfaces
{
    public interface IInstallState
    {
        string ActionName { get; }
        double Progress { get; }
        bool IsProgressIntermidiate { get; }

        bool Action();
    }
}
