using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.WindowsService.IoC;

namespace TiagoViegas.ProPresenterVmixBridge.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;

            var service = IoCManager.CreateContainer().GetInstance<ProPresenterVmixBridgeService>();

            ServicesToRun = new ServiceBase[]
            {
                service
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
