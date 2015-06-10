using MinecraftLauncher.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MinecraftLauncher.Model.Minecraft
{
    [Serializable]
    public class Account
    {
        public string Username { get; set; } 
        public string Login { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
