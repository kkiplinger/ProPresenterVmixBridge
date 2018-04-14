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

            while (!quit)
            {
                var key = System.Console.ReadKey(true);

                switch (key.KeyChar)
                {
                    case 'q':
                        quit = true;
                        break;
                    case 'c':
                        if (!bridgeBc.BridgeOn && !bridgeBc.Connecting)
                        {
                            bridgeBc.Bridge();
                            System.Console.WriteLine("Press d to disconnect.");
                        }
                        else
                        {
                            System.Console.WriteLine("Bridge already connected or connecting.");
                        }
                        break;
                    case 'd':
                        if (bridgeBc.BridgeOn && !bridgeBc.Connecting)
                        {
                            bridgeBc.Close();
                            System.Console.WriteLine("Press c to connect.");
                        }
                        else
                        {
                            System.Console.WriteLine("Bridge not connected or connecting.");
                        }
                        break;
                    default:
                        if (!bridgeBc.Connecting)
                        {
                            if (!bridgeBc.BridgeOn)
                            {
                                System.Console.WriteLine("Press c to connect and q to quit.");
                            }
                            else
                            {
                                System.Console.WriteLine("Press d to disconnect and q to quit.");
                            }
                        }
                        else
                        {
                            System.Console.WriteLine("Wait for connection.");
                        }

                        break;

                }
            }
        }
    }
}
