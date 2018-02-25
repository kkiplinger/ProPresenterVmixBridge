using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiagoViegas.ProPresenterVmixBridge.Data.Interfaces
{
    public interface IConfigManager
    {
        string GetConfig(string key);
        void EditConfig(string key, string value);
        void LoadConfig();
        void SaveConfig();
    }
}
