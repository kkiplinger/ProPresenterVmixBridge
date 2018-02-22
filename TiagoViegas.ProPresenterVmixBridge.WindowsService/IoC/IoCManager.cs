using SimpleInjector;
using System;
using System.Collections.Generic;
using TiagoViegas.ProPresenterVmixBridge.Business.IoC;
using TiagoViegas.ProPresenterVmixBridge.Data.IoC;

namespace TiagoViegas.ProPresenterVmixBridge.WindowsService.IoC
{
    public class IoCManager
    {
        public static Container CreateContainer()
        {
            var container = new Container();

            BusinessIocManager.BootstrapBusiness(container);
            DataIocManager.BootstrapData(container);

            return container;
        }
    }
}
