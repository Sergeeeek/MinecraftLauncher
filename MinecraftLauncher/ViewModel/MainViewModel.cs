using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.ServiceLocation;
using MinecraftLauncher.Model;
using MinecraftLauncher.Model.Minecraft;
using MinecraftLauncher.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MinecraftLauncher.Services.Minecraft;

namespace MinecraftLauncher.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Properties


        private bool _IsSettingsOpen;
        public bool IsSettingsOpen
        {
            get { return _IsSettingsOpen; }
            set
            {
                _IsSettingsOpen = value;
                RaisePropertyChanged("IsSettingsOpen");
            }
        }

        bool _IsSettingsChanged;
        public bool IsSettingsChanged
        {
            get { return _IsSettingsChanged; }
            set
            {
                _IsSettingsChanged = value;
                RaisePropertyChanged("IsSettingsChanged");
            }
        }

        string _SettingsGameFolder;
        public string SettingsGameFolder
        {
            get { return _SettingsGameFolder; }
            set
            {
                _SettingsGameFolder = value;
                IsSettingsChanged = true;
                RaisePropertyChanged("SettingsGameFolder");
            }
        }

        string _SettingsLaunchArguments;
        public string SettingsLaunchArguments
        {
            get { return _SettingsLaunchArguments; }
            set
            {
                _SettingsLaunchArguments = value;
                IsSettingsChanged = true;
                RaisePropertyChanged("SettingsLaunchArguments");
            }
        }


        IGameInstaller _GameInstallerService;
        public IGameInstaller GameInstallerService
        {
            get { return _GameInstallerService; }
            private set
            {
                _GameInstallerService = value;
                RaisePropertyChanged("GameInstallerService");
            }
        }

        IAuthorizationService _Authorization;
        public IAuthorizationService Authorization
        {
            get { return _Authorization; }
            private set
            {
                _Authorization = value;
                RaisePropertyChanged("Authorization");
            }
        }


        #endregion

        #region Commands

        public ICommand Login { get; private set; }

        public ICommand Logout { get; private set; }

        public ICommand Play { get; private set; }

        public ICommand SaveSettings { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            GameInstallerService = ServiceLocator.Current.GetInstance<IGameInstaller>();
            Authorization = ServiceLocator.Current.GetInstance<IAuthorizationService>();

            var instances = ServiceLocator.Current.GetAllInstances<ISaveable>().ToList();

            Init();

            Login = new RelayCommand(async () =>
                {
                    var dial = ServiceLocator.Current.GetInstance<IDialogService>();
                    var data = await  dial.ShowLoginDialog("Вход", "Введите свои данные");
                    if (data == null)
                        return;

                   await Authorization.Login(data.Username, data.Password);

                    if (!Authorization.IsLoggedIn)
                    {
                        var res = await dial.ShowMessageDialog("Ошибка",
                            "Данного логина или пароля не существует. Попробовать ещё раз?",
                            MessageDialogStyle.AffirmativeAndNegative);

                        if (res == MessageDialogResult.Affirmative)
                        {
                            Login.Execute(null);
                        }
                    }
                });

            Logout = new RelayCommand(() =>
            {
                Authorization.Logout();
            });

            Play = new RelayCommand(() =>
            {
                
            });

            SaveSettings = new RelayCommand(async () =>
            {
                IsSettingsChanged = false;
                var set = ServiceLocator.Current.GetInstance<ISettingsService>();

                await set.Save();
                //await GameInstallerService.MoveToFolder(GameInstallerService.InstallFolder);
            });
        }

        async void Init()
        {
            var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            await settingsService.Load();

            while (!settingsService.Loaded)
                await Task.Delay(100);
        }

        public void OnWindowClosing(object sender, CancelEventArgs args)
        {
            //if (SettingsService is ISaveable)
            //{
            //    SaveSettings.Execute(null);
            //    //var task = Task.Run(() => (SettingsService as ISaveable).Save());
            //    //task.Wait();
            //}

            //if (Authorization is ISaveable)
            //{
            //    var task = Task.Run(() => (Authorization as ISaveable).Save());
            //    task.Wait();
            //}

            //if (GameService is ISaveable)
            //{
            //    var task = Task.Run(() => (GameService as ISaveable).Save());
            //    task.Wait();
            //}

            var set = ServiceLocator.Current.GetInstance<ISettingsService>();

            set.Save().Wait();
        }

        
    }
}