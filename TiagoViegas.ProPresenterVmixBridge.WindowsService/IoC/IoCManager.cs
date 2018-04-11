using SimpleInjector;
using System;
using System.Collections.Generic;
using TiagoViegas.ProPresenterVmixBridge.Business.IoC;
using TiagoViegas.ProPresenterVmixBridge.Data.IoC;
using TiagoViegas.ProPresenterVmixBridge.Shared.Ioc;

namespace TiagoViegas.ProPresenterVmixBridge.WindowsService.IoC
{
    public class IoCManager
    {
        public static Container CreateContainer()
        {
            var container = new Container();

            container.Register<ProPresenterVmixBridgeService>(Lifestyle.Transient);

            BusinessIocManager.RegisterDependenciesInto(container);
            DataIocManager.RegisterDependenciesInto(container);
            SharedIocManager.RegisterDependenciesInto(container);

            return container;
        }
    }
}
