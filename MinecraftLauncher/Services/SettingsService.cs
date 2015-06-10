using Microsoft.Practices.ServiceLocation;
using MinecraftLauncher.Model;
using MinecraftLauncher.Model.Minecraft;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinecraftLauncher.Services
{
    public class SettingsService : ISettingsService
    {
        public bool Loaded { get; private set; }

        public async Task Save()
        {
            var types = GetServiceInterfaces();

            var sets = new List<SettingsEntry>();

            foreach (var t in types)
            {
                try
                {
                    var service = ServiceLocator.Current.GetInstance(t) as ISaveable;
                    if (service == null)
                        continue;

                    var sav = await service.Save();

                    sets.Add(new SettingsEntry() { ServiceType = t, Entry = sav });
                }
                catch
                {

                }
            }

            using (var file = File.Create("settings.dat"))
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize(file, sets);
            }
        }

        List<Type> GetServiceInterfaces()
        {
            var saveable = typeof(ISaveable);
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(z => z.Namespace == "MinecraftLauncher.Services" && z.IsInterface);

            return types.ToList();
        }

        public async Task Load()
        {
            List<SettingsEntry> entries = null;

            try
            {
                using (var file = File.OpenRead("settings.dat"))
                {
                    var formatter = new BinaryFormatter();

                    entries = formatter.Deserialize(file) as List<SettingsEntry>;
                }
            }
            catch
            {
                entries = (from t in GetServiceInterfaces()
                           select new SettingsEntry() { ServiceType = t, Entry = null }).ToList();
            }

            foreach (var entry in entries)
            {
                try
                {
                    var service = ServiceLocator.Current.GetInstance(entry.ServiceType) as ISaveable;
                    if (service == null)
                        continue;

                    await service.Load(entry.Entry);
                }
                catch
                {

                }
            }

            Loaded = true;
        }
    }
    [Serializable]
    class SettingsEntry
    {
        public Type ServiceType { get; set; }
        public object Entry { get; set; }
    }
}
