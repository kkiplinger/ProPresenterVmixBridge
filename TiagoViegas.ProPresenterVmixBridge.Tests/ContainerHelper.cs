using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Business.IoC;
using TiagoViegas.ProPresenterVmixBridge.Data.IoC;

namespace TiagoViegas.ProPresenterVmixBridge.Tests
{
    public class ContainerHelper
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
