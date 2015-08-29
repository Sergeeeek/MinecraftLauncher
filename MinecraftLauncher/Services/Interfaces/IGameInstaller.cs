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
    public interface IGameInstaller
    {
        InstallState CurrentState { get; }
        double Progress { get; }
        bool IsIntermidiate { get; }
        string DownloadFolder { get; }
        string InstallFolder { get; }

        /// <summary>
        /// Downloads the game
        /// </summary>
        /// <param name="path">where to put downloaded file into</param>
        /// <returns></returns>
        Task Download(Uri uri, string path);
        /// <summary>
        /// Installs the game into the path + Game name
        /// </summary>
        /// <returns></returns>
        Task Install(string path);
        /// <summary>
        /// Removes the game
        /// </summary>
        /// <returns></returns>
        Task Remove();
        /// <summary>
        /// Moves the game to the spcified folder
        /// </summary>
        /// <returns></returns>
        Task MoveToFolder(string newPath);
    }
}
