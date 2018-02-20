
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

        public ConfigManager()
        {
            LoadConfig();
        }

        public void EditConfig(string key, string value)
        {
            var config = Configs.First(x => x.Key == key);

            config.Value = value;

            SaveConfig();
            LoadConfig();
        }

        public string GetConfig(string key)
        {
            return Configs.First(x => x.Key == key).Value;
        }

        private void LoadConfig()
        {
            var path = System.Environment.CurrentDirectory;

            var configText = File.ReadAllText(Path.Combine(path, ConfigFileName));

            Configs = JsonConvert.DeserializeObject<IEnumerable<Config>>(configText);
        }

        private void SaveConfig()
        {
            var text = JsonConvert.SerializeObject(Configs);

            var path = System.Environment.CurrentDirectory;

            File.WriteAllText(Path.Combine(path, ConfigFileName), text);
        }
    }
}
