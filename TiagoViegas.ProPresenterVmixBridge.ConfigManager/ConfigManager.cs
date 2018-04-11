using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TiagoViegas.ProPresenterVmixBridge.Configuration;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Logging;

namespace TiagoViegas.ProPresenterVmixBridge.ConfigManager
{
    public class ConfigManager : IConfigManager
    {
        private const string ConfigFileName = "config.json";
        private ICollection<Config> _configs;
        private readonly string _currentPath;
        private readonly object _lock = new object();
        private readonly ILogger _logger;

        public ConfigManager(ILogger logger)
        {
            _logger = logger;
            _configs = new List<Config>();
            _currentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ProPresenter Vmix Bridge");
            LoadConfig();
        }

        public void EditConfig(string key, string value)
        {
            var config = _configs.FirstOrDefault(x => x.Key == key);

            if (config == null)
            {
                config = new Config
                {
                    Key = key
                };
                _configs.Add(config);
            }

            config.Value = value;
        }

        public string GetConfig(string key)
        {
            return _configs.First(x => x.Key == key).Value;
        }

        public void LoadConfig()
        {
                if (!Directory.Exists(_currentPath))
                {
                    Directory.CreateDirectory(_currentPath);
                }

                var configFile = Path.Combine(_currentPath, ConfigFileName);

                if (!File.Exists(configFile))
                {
                    _logger.LogInfoFormat("Creating config file: {0}", configFile);
                    File.Create(configFile);
                }
               
                _logger.LogInfoFormat("Loading config file: {0}", configFile);

                
                var configText = File.ReadAllText(configFile);
                _configs = JsonConvert.DeserializeObject<ICollection<Config>>(configText);
                

                if (_configs == null)
                {
                    _configs = new List<Config>();
                    SaveConfig();
                }
        }

        public void SaveConfig()
        {
            
                var text = JsonConvert.SerializeObject(_configs);

                File.WriteAllText(Path.Combine(_currentPath, ConfigFileName), text);
            
        }
    }
}
