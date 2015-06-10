using MinecraftLauncher.Model;
using MinecraftLauncher.Model.Minecraft;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MinecraftLauncher.Services
{
    public class GameSettingsService : IGameSettingsService, ISaveable, INotifyPropertyChanged
    {
        string _GameFolder;
        public string GameFolder
        {
            get { return _GameFolder; }
            set
            {
                _GameFolder = value;
                RaisePropertyChanged("GameFolder");
            }
        }

        string _Arguments;
        public string Arguments
        {
            get { return _Arguments; }
            set
            {
                _Arguments = value;
                RaisePropertyChanged("Arguments");
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

        public async Task Load(object obj)
        {
            var sett = obj as SavedGameSettings;

            if (sett == null)
                return;

            GameFolder = sett.GameFolder ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ".warzone_minecraft");
            Arguments = sett.Arguments ?? "-Xms1G -Xmx1G";
        }

        public async Task<object> Save()
        {
            return new SavedGameSettings() { GameFolder = GameFolder, Arguments = Arguments };
        }
    }
}
