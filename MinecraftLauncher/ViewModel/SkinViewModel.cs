using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using MinecraftLauncher.Model;
using MinecraftLauncher.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftLauncher.ViewModel
{
    public class SkinViewModel : ViewModelBase
    {
        public string SkinImageUrl
        {
            get 
            {
                var auth = ServiceLocator.Current.GetInstance<IAuthorizationService>();
                
                return string.Format(skinImageUrlBase, auth.CurrentAccount != null && auth.IsLoggedIn ? auth.CurrentAccount.Username : "");
            }
        }

        string skinImageUrlBase = "http://mc.warzone.su/skin.php?user_name={0}";

        public SkinViewModel()
        {
            var auth = ServiceLocator.Current.GetInstance<IAuthorizationService>();
            auth.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == "CurrentAccount") RaisePropertyChanged("SkinImageUrl");
            };
        }
    }
}
