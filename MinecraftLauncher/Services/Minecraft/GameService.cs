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

namespace MinecraftLauncher.Services.Minecraft
{
    public class GameService : IGameService, ISaveable, INotifyPropertyChanged
    {
        Process process;

        GameState _CurrentState;
        public GameState CurrentState
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

        public GameService()
        {
            webClient = new WebClient();
            webClient.DownloadProgressChanged += (o, e) => Progress = e.ProgressPercentage;
        }

        public void Action()
        {
            switch (CurrentState)
            {
                case GameState.NotInstalled:
                    Install();
                    break;
                case GameState.Downloading:
                    CancelDownload();
                    break;
                case GameState.Installed:
                    Check();
                    break;
                case GameState.Checking:
                    //cancel
                    break;
                case GameState.Playing:
                    Close();
                    break;
                default:
                    break;
            }
        }

        public async Task ChangeFolder(string newPath)
        {
            string path = ServiceLocator.Current.GetInstance<IGameSettingsService>().GameFolder;
            if (newPath == path || CurrentState == GameState.NotInstalled)
                return;

            try
            {
                await Task.Run(() => Directory.Move(path, newPath));
            }
            catch
            {

            }
        }

        async void Install()
        {
            string path = ServiceLocator.Current.GetInstance<IGameSettingsService>().GameFolder;

            CurrentState = GameState.Downloading;
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

                        CurrentState = GameState.Installed;
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

                        CurrentState = GameState.NotInstalled;
                    }
                });
            }
            catch (Exception ex)
            {
                if (!(ex is WebException) && (ex as WebException).Status != WebExceptionStatus.RequestCanceled)
                    ServiceLocator.Current.GetInstance<IDialogService>().ShowMessageDialog("Ошибка", ex.Message, MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative);

                CurrentState = GameState.NotInstalled;
            }
        }

        void CancelDownload()
        {
            downloadCts.Cancel();
            downloadCts.Dispose();
            CurrentState = GameState.NotInstalled;
            Progress = 0;
        }

        async void Check()
        {
            bool checkSucceded = true;
            try
            {
                CurrentState = GameState.Checking;
                var folder = ServiceLocator.Current.GetInstance<IGameSettingsService>().GameFolder;
                checkCts = new CancellationTokenSource();
                checkCts.Token.ThrowIfCancellationRequested();

                //var hash = "4ae4311b62fe82439e584d4f3df12895";
                var hash = await webClient.DownloadStringTaskAsync("http://warzone.su/games/MinecraftClient/minecraft.md5");

                byte[] arr;

                using (var file = File.OpenRead(Path.Combine(folder, @"bin\minecraft.jar")))
                using (var md5 = MD5.Create())
                {
                    arr = md5.ComputeHash(file);
                }

                var sb = new StringBuilder();
                for (int i = 0; i < arr.Length; i++)
                {
                    sb.Append(arr[i].ToString("X2"));
                }

                if (hash.ToUpper() == sb.ToString().ToUpper()) // Need to be sure strings are uppercase to match
                {
                    Play();
                }
                else
                {
                    CurrentState = GameState.NotInstalled;
                }
            }
            catch
            {
                checkSucceded = false;
            }

            if (!checkSucceded)
            {
                var dial = ServiceLocator.Current.GetInstance<IDialogService>();
                await dial.ShowMessageDialog("Ошибка", "Ошибка при проверке игры, нужно переустановить её.", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative);
            }
        }

        async void Play()
        {
            var acc = ServiceLocator.Current.GetInstance<IAuthorizationService>();
            if (!acc.IsLoggedIn)
            {
                var dial = ServiceLocator.Current.GetInstance<IDialogService>();
                var res = await dial.ShowMessageDialog("Ошибка", "Вы не совершили вход в систему. Войти сейчас?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative);
                if (res == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                {
                    var loc = Application.Current.Resources["Locator"] as ViewModelLocator;
                    loc.Main.Login.Execute(null);
                }
                else
                {
                    return;
                }
            }
            CurrentState = GameState.Playing;
            var rk = Registry.LocalMachine;
            object currentVerion;
            using (var subKey = rk.OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment"))
            {
                currentVerion = subKey.GetValue("CurrentVersion");
            }

            if (currentVerion == null)
            {
                var dial = ServiceLocator.Current.GetInstance<IDialogService>();

                var result = await dial.ShowMessageDialog("Ошибка", "На вашем компьютере не установлен Java Runtime. Хотите установить его сейчас?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative);

                if (result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Negative)
                    return;

                await InstallJava();
            }

            CurrentState = GameState.Installed;

            //{0} - Game folder
            //{1} - username
            //{2} - token
            //{3} - server ip
            //{4} - server port
            //{5} - arguments
            var settings = ServiceLocator.Current.GetInstance<IGameSettingsService>();
            string path = settings.GameFolder;


            var procInfo = new ProcessStartInfo("javaw.exe", string.Format("{5} -Dfml.ignorePatchDiscrepancies=true -Dfml.ignoreInvalidMinecraftCertificates=true -Djava.library.path=\"{0}\\bin\\natives\" -cp \"{0}\\bin\\*\" net.minecraft.launchwrapper.Launch --tweakClass cpw.mods.fml.common.launcher.FMLTweaker --username {1} --session {2} --server {3} --port {4} --version 1.6.4 --gameDir \"{0}\" --assetsDir \"{0}\\assets\"", path, acc.CurrentAccount.Username, acc.CurrentAccount.Token, "127.0.0.1", "25565", settings.Arguments));

            using (process = new Process())
            {
                process.StartInfo = procInfo;
                process.Start();
                CurrentState = GameState.Playing;
                await Task.Run(() => process.WaitForExit());
            }

            process = null;
            CurrentState = GameState.Installed;
        }

        private void Close()
        {
            if (process != null) process.Kill();
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
            if (!(obj is GameState) || obj == null)
            {
                CurrentState = GameState.NotInstalled;
                IsActionEnabled = true;
                return;
            }

            CurrentState = (GameState)obj;
            IsActionEnabled = true;
        }

        public async Task<object> Save()
        {
            if (process != null)
            {
                process.Kill();
            }

            return (int)CurrentState > 1 ? GameState.Installed : GameState.NotInstalled;
        }
        #endregion
    }
}
