using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Business.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;

namespace TiagoViegas.ProPresenterVmixBridge.Business
{
    public class BridgeBc : IBridgeBc
    {
        private readonly IVmixDataAgent _vmixDA;
        private readonly IProPresenterDataAgent _proPresenterDA;

        public bool BridgeOn { get; set; }

        public BridgeBc(IProPresenterDataAgent proPresenterDataAgent, IVmixDataAgent vmixDataAgent)
        {
            _vmixDA = vmixDataAgent;
            _proPresenterDA = proPresenterDataAgent;
            BridgeOn = false;
        }


        public void Bridge()
        {
            var connectionRetries = 0;
            while (!_proPresenterDA.Connected)
            {
                if (!_proPresenterDA.Connecting)
                {
                    _proPresenterDA.Connect();
                    connectionRetries++;
                }
                if(connectionRetries >= 10)
                {
                    break;
                }
            }

            if (_proPresenterDA.Connected)
            {
                _proPresenterDA.Listen((sender, e) =>
                {
                    var text = e.Data;
                    Console.WriteLine(text);

                    _vmixDA.SendText(text);
                });

                BridgeOn = true;
            }
            
        }

        public void Close()
        {
            _proPresenterDA.Close();
            BridgeOn = false;
        }
    }
}
