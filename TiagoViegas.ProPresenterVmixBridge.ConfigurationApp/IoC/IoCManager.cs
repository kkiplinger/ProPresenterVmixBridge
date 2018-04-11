using SimpleInjector;
using TiagoViegas.ProPresenterVmixBridge.Business.IoC;
using TiagoViegas.ProPresenterVmixBridge.Data.IoC;
using TiagoViegas.ProPresenterVmixBridge.Shared.Ioc;

namespace TiagoViegas.ProPresenterVmixBridge.ConfigurationApp.IoC
{
    public class IoCManager
    {
        public static Container CreateContainer()
        {
            var container = new Container();

            BusinessIocManager.RegisterDependenciesInto(container);
            DataIocManager.RegisterDependenciesInto(container);
            SharedIocManager.RegisterDependenciesInto(container);

            return container;
        }
    }
}
