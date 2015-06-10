using MinecraftLauncher.Model.Minecraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftLauncher.Services
{
    /// <summary>
    /// Service which automaticaly saves and loads all ISaveables through a neat interface :)
    /// </summary>
    public interface ISettingsService
    {
        bool Loaded { get; }

        Task Save();
        Task Load();
    }
}
