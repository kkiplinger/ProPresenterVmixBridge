using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Business.Interfaces;

namespace TiagoViegas.ProPresenterVmixBridge.Business.IoC
{
    public class BusinessIocManager
    {
        public static void RegisterDependenciesInto(Container container)
        {
            container.Register<IBridgeBc, BridgeBc>(Lifestyle.Transient);
        }
    }
}
