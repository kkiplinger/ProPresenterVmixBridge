using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Configuration;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.DataAgents;

namespace TiagoViegas.ProPresenterVmixBridge.Data.IoC
{
    public class DataIocManager
    {
        public static void BootstrapData(Container container)
        {
            container.Register<IConfigManager, ConfigManager>(Lifestyle.Singleton);
            container.Register<IProPresenterDataAgent, ProPresenterDataAgent>(Lifestyle.Transient);
            container.Register<IVmixDataAgent, VmixDataAgent>(Lifestyle.Transient);
        }
    }
}
