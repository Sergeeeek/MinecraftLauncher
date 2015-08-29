/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:MinecraftLauncher"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using MinecraftLauncher.Model;
using MinecraftLauncher.Services;
using MinecraftLauncher.Services.Minecraft;

namespace MinecraftLauncher.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                //SimpleIoc.Default.Register<IDataService, DesignDataService>();
                SimpleIoc.Default.Register<IDialogService, DialogService>();
                SimpleIoc.Default.Register<IAuthorizationService, AuthorizationService>();
                SimpleIoc.Default.Register<IGameInstaller, MinecraftGameInstaller>();
                //SimpleIoc.Default.Register<IGameSettingsService, GameSettingsService>();
                SimpleIoc.Default.Register<ISettingsService, BinarySettingsService>();
            }
            else
            {
                // Create run time view services and models
                SimpleIoc.Default.Register<IDialogService, DialogService>();
                SimpleIoc.Default.Register<IAuthorizationService, AuthorizationService>();
                SimpleIoc.Default.Register<IGameInstaller, MinecraftGameInstaller>();
                //SimpleIoc.Default.Register<IGameSettingsService, GameSettingsService>();
                SimpleIoc.Default.Register<ISettingsService, BinarySettingsService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SkinViewModel>();
            SimpleIoc.Default.Register<NewsViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public SkinViewModel Skin
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SkinViewModel>();
            }
        }

        public NewsViewModel News
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NewsViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}