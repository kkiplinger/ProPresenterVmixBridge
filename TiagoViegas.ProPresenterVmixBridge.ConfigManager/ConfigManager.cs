
using System.IO;
using System.Collections.Generic;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using Newtonsoft.Json;
using System.Linq;

namespace TiagoViegas.ProPresenterVmixBridge.Configuration
{
    public class ConfigManager : IConfigManager
    {
        private const string ConfigFileName = "config.json";
        private IEnumerable<Config> Configs;
        private readonly string _currentPath;

        public ConfigManager()
        {
            Configs = new List<Config>();
            _currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
            LoadConfig();
        }

        public void EditConfig(string key, string value)
        {
            var config = Configs.First(x => x.Key == key);

            config.Value = value;

            
        }

        public string GetConfig(string key)
        {
            return Configs.First(x => x.Key == key).Value;
        }

        public void LoadConfig()
        {
            var configFile = Path.Combine(_currentPath, ConfigFileName);

            if (!File.Exists(configFile))
            {
                File.Create(configFile);
            }
            else
            {
                var configText = File.ReadAllText(configFile);
                Configs = JsonConvert.DeserializeObject<IEnumerable<Config>>(configText);
            }
        }

        public void SaveConfig()
        {
            var text = JsonConvert.SerializeObject(Configs);

            File.WriteAllText(Path.Combine(_currentPath, ConfigFileName), text);
        }
    }
}
