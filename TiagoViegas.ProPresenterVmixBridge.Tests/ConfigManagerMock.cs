using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Entities;

namespace TiagoViegas.ProPresenterVmixBridge.Tests
{
    class ConfigManagerMock : IConfigManager
    {


        public void EditConfig(string key, string value)
        {
            throw new NotImplementedException();
        }

        public string GetConfig(string key)
        {
            switch (key)
            {
                case ConfigKeys.ProPresenterName:
                    return "PROPRESENTER";
                case ConfigKeys.ProPresenterIp:
                    return "192.168.1.68";
                case ConfigKeys.ProPresenterPassword:
                    return "123";
                case ConfigKeys.ProPresenterPort:
                    return "50001";
                case ConfigKeys.VmixIp:
                    return "127.0.0.1";
                case ConfigKeys.VmixPort:
                    return "8088";
                case ConfigKeys.VmixInputNumber:
                    return "1";
            }
            throw new InvalidOperationException();
        }

        public void LoadConfig()
        {
            throw new NotImplementedException();
        }

        public void SaveConfig()
        {
            throw new NotImplementedException();
        }
    }
}
