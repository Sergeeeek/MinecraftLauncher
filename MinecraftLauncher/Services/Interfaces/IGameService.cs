using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using MinecraftLauncher.Extensions;
using MinecraftLauncher.Model;
using MinecraftLauncher.ViewModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace MinecraftLauncher.Services
{
    public interface IGameService
    {
        GameState CurrentState { get; }
        double Progress { get; }
        bool IsIntermidiate { get; }

        void Action();
        Task ChangeFolder(string newPath);
    }
}
