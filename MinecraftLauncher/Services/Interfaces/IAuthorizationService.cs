using MinecraftLauncher.Model;
using MinecraftLauncher.Model.Minecraft;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftLauncher.Services
{
    public interface IAuthorizationService
    {
        Account CurrentAccount { get; }
        bool IsLoggedIn { get; }

        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Function which will try to login with given params
        /// </summary>
        /// <param name="user">username</param>
        /// <param name="pass">password</param>
        /// <returns>Model.Account or null</returns>
        Task Login(string user, string pass);

        void Logout();
    }
}
