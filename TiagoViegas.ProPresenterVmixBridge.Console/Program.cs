using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Business.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Console.IoC;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;

namespace TiagoViegas.ProPresenterVmixBridge.Console
{
    class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            var container = IoCManager.CreateContainer();

            var bridgeBc = container.GetInstance<IBridgeBc>();

            var quit = false;

            System.Console.WriteLine("Press c to connect and q to quit.");

            bridgeBc.Bridge();

            while (true) {
            }
        }
    }
}
