
using MinecraftLauncher.Model;
using MinecraftLauncher.Model.Minecraft;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MinecraftLauncher.Services
{
    public interface IGameSettingsService
    {
        string GameFolder { get; }
        string Arguments { get; }
    }
}
