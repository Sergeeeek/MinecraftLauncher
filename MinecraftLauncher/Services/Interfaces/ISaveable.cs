using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftLauncher.Services
{
    public interface ISaveable
    {
        Task Load(object obj);
        Task<object> Save();
    }
}
