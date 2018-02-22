using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Business.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Entities;

namespace TiagoViegas.ProPresenterVmixBridge.Business
{
    public class BridgeBc : IBridgeBc
    {
        private readonly IVmixDataAgent _vmixDA;
        private readonly IProPresenterDataAgent _proPresenterDA;

        public bool BridgeOn { get; set; }
        public bool Connecting { get; set; }

        public BridgeBc(IProPresenterDataAgent proPresenterDataAgent, IVmixDataAgent vmixDataAgent)
        {
            _vmixDA = vmixDataAgent;
            _proPresenterDA = proPresenterDataAgent;
            BridgeOn = false;
        }


        public async Task Bridge()
        {
            if (BridgeOn)
            {
                return;
            }

            Connecting = true;

            var cts = new CancellationTokenSource();

            await _proPresenterDA.Connect(cts.Token);

            if (_proPresenterDA.Connected)
            {
                Console.WriteLine("Connected");
                _proPresenterDA.Listen((x) =>
                {
                    var text = x.Array.FirstOrDefault(y => y.Action == ProPresenterActions.CurrentSlide).Text;
                    Console.WriteLine(text.Trim('\n'));
                    _vmixDA.SendText(text.Trim('\n'));
                });

                BridgeOn = true;
            }
            else
            {
                cts.Cancel();
                Console.WriteLine("Could not connect");
                BridgeOn = false;
            }
            Connecting = false;
        }

        public async Task Close()
        {
            if (!BridgeOn)
            {
                return;
            }

            _proPresenterDA.StopListen();
            await _proPresenterDA.Close();
            BridgeOn = false;
        }
    }
}
