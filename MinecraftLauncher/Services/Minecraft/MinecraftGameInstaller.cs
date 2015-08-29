using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using MinecraftLauncher.Model;
using MinecraftLauncher.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using MinecraftLauncher.Extensions;
using System.Diagnostics.Contracts;

namespace MinecraftLauncher.Services.Minecraft
{
    public sealed class MinecraftGameInstaller : IGameInstaller, ISaveable, INotifyPropertyChanged
    {
        InstallState _CurrentState;
        public InstallState CurrentState
        {
            get { return _CurrentState; }
            private set
            {
                _CurrentState = value;
                RaisePropertyChanged("CurrentState");
            }
        }

        double _Progress;
        public double Progress
        {
            get { return _Progress; }
            set
            {
                _Progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        bool _IsIntermidiate;
        public bool IsIntermidiate
        {
            get { return _IsIntermidiate; }
            set
            {
                _IsIntermidiate = value;
                RaisePropertyChanged("IsIntermidiate");
            }
        }

        string _DownloadFolder;
        public string DownloadFolder
        {
            get { return _DownloadFolder; }
            private set
            {
                _DownloadFolder = value;
                RaisePropertyChanged("DownloadFolder");
            }
        }

        string _InstallFolder;
        public string InstallFolder
        {
            get { return _InstallFolder; }
            private set
            {
                _InstallFolder = value;
                RaisePropertyChanged("InstallFolder");
            }
        }

        bool _IsActionEnabled;
        public bool IsActionEnabled
        {
            get { return _IsActionEnabled; }
            set
            {
                _IsActionEnabled = value;
                RaisePropertyChanged("IsActionEnabled");
            }
        }

        WebClient webClient;
        CancellationTokenSource downloadCts;
        CancellationTokenSource checkCts;

        public MinecraftGameInstaller()
        {
            webClient = new WebClient();
            webClient.DownloadProgressChanged += (o, e) => Progress = e.ProgressPercentage;
        }

        public async Task MoveToFolder(string newPath)
        {
            string path = DownloadFolder;

            if (newPath == path || CurrentState < InstallState.Installed)
                return;

            try
            {
                await Task.Run(() => Directory.Move(path, newPath));
            }
            catch
            {

            }
        }

        async Task Install()
        {
            string path = DownloadFolder;

            CurrentState = InstallState.Downloading;
            downloadCts = new CancellationTokenSource();
            try
            {
                if (!File.Exists("minecraft.zip"))
                    await webClient.DownloadFileTaskAsync("http://warzone.su/games/MinecraftClient/minecraft.zip", "minecraft.zip", downloadCts.Token);

                IsIntermidiate = true;

                await Task.Run(() =>
                {
                    try
                    {
                        if (Directory.Exists(path))
                            Directory.Delete(path, true);

                        using (var file = File.OpenRead("minecraft.zip"))
                        using (var zip = new ZipArchive(file))
                        {
                            zip.ExtractToDirectory(path);
                        }
                        downloadCts.Token.ThrowIfCancellationRequested();

                        IsIntermidiate = false;
                        Progress = 0;

                        CurrentState = InstallState.Installed;
                    }
                    catch
                    {
                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);
                        }
                        if (File.Exists("minecraft.zip"))
                        {
                            File.Delete("minecraft.zip");
                        }

                        IsIntermidiate = false;
                        Progress = 0;

                        CurrentState = InstallState.NotInstalled;
                    }
                });
            }
            catch (Exception ex)
            {
                CurrentState = InstallState.NotInstalled;

                if (!(ex is WebException))
                {
                    if ((ex as WebException).Status != WebExceptionStatus.RequestCanceled)
                    {
                        throw;
                    }
                }

                throw;
            }
        }

        public async Task Download(Uri uri)
        {
            Contract.Requires<ArgumentNullException>(uri != null || string.IsNullOrWhiteSpace(DownloadFolder));

            if (CurrentState > InstallState.NotInstalled)
                return;

            DownloadFolder = Path.Combine(path, Path.GetFileName(uri.AbsolutePath));

            downloadCts = new CancellationTokenSource();

            try
            {
                CurrentState = InstallState.Downloading;
                await webClient.DownloadFileTaskAsync(uri.ToString(), DownloadFolder, downloadCts.Token);

                CurrentState = InstallState.Downloaded;
            }
            catch (OperationCanceledException ex) // Don't wanna throw if we just cancelled
            {
                CurrentState = InstallState.NotInstalled;
            }
            catch (Exception ex)
            {
                this.Log().Error("Download failed.", ex);
                CurrentState = InstallState.NotInstalled;
                throw;
            }
            finally
            {
                downloadCts.Dispose();
            }
        }

        public async Task Install(string path)
        {
            Contract.Requires<ArgumentNullException>(string.IsNullOrWhiteSpace(path));

            if (CurrentState != InstallState.Downloaded)
                return;

            InstallFolder = Path.Combine(path, "warzone_minecraft");
            ZipArchive archive;

            try
            {
                try
                {
                    archive = ZipFile.OpenRead(DownloadFolder);
                }
                catch (Exception ex)
                {
                    this.Log().Error("Can't open game's archive.", ex);
                    throw;
                }

                try
                {
                    await Task.Run(() => archive.ExtractToDirectory(InstallFolder));
                }
                catch (Exception ex)
                {
                    this.Log().Error("Can't extract archive.", ex);
                    throw;
                }

                CurrentState = InstallState.Installed;
            }
            catch
            {
                this.Log().Error("Installation failed.");
                CurrentState = InstallState.Downloaded;
                throw;
            }
        }

        public async Task Remove()
        {
            if (CurrentState != InstallState.Installed)
                return;

            try
            {
                await Task.Run(() => Directory.Delete(InstallFolder));
                CurrentState = InstallState.NotInstalled;
            }
            catch (Exception ex)
            {
                this.Log().Error("Can't remove the game.", ex);
                throw;
            }
        }

        public void CancelDownload()
        {
            downloadCts.Cancel();
            downloadCts.Dispose();
            CurrentState = InstallState.NotInstalled;
            Progress = 0.0;

            this.Log().Info("Cancelled download.");
        }

        async Task InstallJava()
        {
            if (!File.Exists("javainstall.exe"))
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    await webClient.DownloadFileTaskAsync("http://javadl.sun.com/webapps/download/AutoDL?BundleId=95125", "java.exe");
                }
                else
                {
                    await webClient.DownloadFileTaskAsync("http://javadl.sun.com/webapps/download/AutoDL?BundleId=95123", "java.exe");
                }
            }

            IsIntermidiate = true;
            await Task.Factory.StartNew(() =>
            {
                var info = new ProcessStartInfo("javainstall.exe", "/s"); //installing java in silent mode
                using (var proc = Process.Start(info))
                {
                    proc.WaitForExit();
                }
            });

            IsIntermidiate = false;
            Progress = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }


        #region ISaveable
        public async Task Load(object obj)
        {
            if (!(obj is GameInstallerSettings) || obj == null)
            {
                CurrentState = InstallState.NotInstalled;
                IsActionEnabled = true;
                return;
            }

            var settings = obj as GameInstallerSettings;
            DownloadFolder = settings.DownloadFolder;
            InstallFolder = settings.InstallFolder;
            CurrentState = settings.State;

            IsActionEnabled = true;
        }

        public async Task<object> Save()
        {
            return new GameInstallerSettings() { DownloadFolder = DownloadFolder, InstallFolder = InstallFolder, State = CurrentState };
        }
        #endregion
    }

    [Serializable]
    public class GameInstallerSettings
    {
        public string DownloadFolder { get; set; }
        public string InstallFolder { get; set; }
        public InstallState State { get; set; }
    }
}
