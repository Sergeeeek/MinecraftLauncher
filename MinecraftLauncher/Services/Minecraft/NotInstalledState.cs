using MinecraftLauncher.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using GalaSoft.MvvmLight.Ioc;

namespace MinecraftLauncher.Services.Minecraft
{
    /// <summary>
    /// In this state game is not installed and action starts the installation
    /// </summary>
    public class NotInstalledState : IInstallState
    {
        IGameService _Service;

        public string ActionName { get; } = "Установить";

        public bool IsProgressIntermidiate { get; } = false;

        public double Progress { get; } = 0;

        public NotInstalledState()
        {
            _Service = SimpleIoc.Default.GetInstance<IGameService>();
        }

        public bool Action()
        {
            
        }
    }
}
