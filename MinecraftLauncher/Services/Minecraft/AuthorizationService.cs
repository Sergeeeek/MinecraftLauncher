using MinecraftLauncher.Model;
using MinecraftLauncher.Model.Minecraft;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MinecraftLauncher.Services.Minecraft
{
    public class AuthorizationService : IAuthorizationService, ISaveable, INotifyPropertyChanged
    {
        public bool Loaded { get; private set; }

        Account _CurrentAccount;
        public Account CurrentAccount
        {
            get { return _CurrentAccount; }
            private set
            {
                _CurrentAccount = value;
                RaisePropertyChanged("CurrentAccount");
            }
        }

        bool _IsLoggedIn;
        public bool IsLoggedIn
        {
            get { return _IsLoggedIn; }
            private set
            {
                _IsLoggedIn = value;
                RaisePropertyChanged("IsLoggedIn");
            }
        }



        public async Task Login(string user, string pass)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    var res = await client.UploadStringTaskAsync("http://mc.warzone.su/MineCraft/auth.php", String.Format("user={0}&password={1}&version=1", user, pass));
                    var response = res.Split(':');

                    if (response.Length != 5)
                    {
                        IsLoggedIn = false;
                        return;
                    }

                    IsLoggedIn = true;
                    CurrentAccount = new Account() { Token = response[3], Login = user, Password = pass, Username = response[2] };
                }
            }
            catch
            {
                IsLoggedIn = false;
            }
        }

        public void Logout()
        {
            if (IsLoggedIn)
            {
                CurrentAccount = null;
                IsLoggedIn = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public async Task<object> Save()
        {
            return CurrentAccount;
        }

        public async Task Load(object obj)
        {
            if (!(obj is Account) || obj == null)
                return;

            var acc = (Account)obj;

            await Login(acc.Login, acc.Password);
        }
    }
}
