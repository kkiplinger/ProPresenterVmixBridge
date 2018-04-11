using SimpleInjector;
using TiagoViegas.ProPresenterVmixBridge.Logging;

namespace TiagoViegas.ProPresenterVmixBridge.Shared.Ioc
{
    public class SharedIocManager
    {
        public static void RegisterDependenciesInto(Container container)
        {
            container.Register<ILogger, Logger>(Lifestyle.Singleton);
        }
    }
}
