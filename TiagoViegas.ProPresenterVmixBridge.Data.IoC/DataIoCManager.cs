using SimpleInjector;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.DataAgents;

namespace TiagoViegas.ProPresenterVmixBridge.Data.IoC
{
    public class DataIocManager
    {
        public static void RegisterDependenciesInto(Container container)
        {
            container.Register<IConfigManager, ConfigManager.ConfigManager>(Lifestyle.Singleton);
            container.Register<IProPresenterDataAgent, ProPresenterDataAgent>(Lifestyle.Singleton);
            container.Register<IVmixDataAgent, VmixDataAgent>(Lifestyle.Transient);
        }
    }
}
