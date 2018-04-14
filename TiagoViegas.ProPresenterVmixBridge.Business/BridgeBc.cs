using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Business.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Entities;
using TiagoViegas.ProPresenterVmixBridge.Logging;

namespace TiagoViegas.ProPresenterVmixBridge.Business
{
    public class BridgeBc : IBridgeBc
    {
        private readonly IVmixDataAgent _vmixDa;
        private readonly IProPresenterDataAgent _proPresenterDa;
        private readonly ILogger _logger;

        public bool BridgeOn { get; set; }
        public bool Connecting { get; set; }
        private Action OnConnectionAction { get; set; }

        public BridgeBc(IProPresenterDataAgent proPresenterDataAgent, IVmixDataAgent vmixDataAgent, ILogger logger)
        {
            _vmixDa = vmixDataAgent;
            _logger = logger;
            _proPresenterDa = proPresenterDataAgent;
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

            try
            {
                _logger.LogInfo("Connecting");
                await _proPresenterDa.Connect(cts.Token);
            }catch (Exception e)
            {
                _logger.LogError("Error while connecting", e);
                Connecting = false;
                return;
            }
            

            if (_proPresenterDa.Connected)
            {
                _logger.LogInfo("Connected");

                OnConnectionAction?.Invoke();

                _proPresenterDa.Listen((x) =>
                {
                    var slide = x.Array.FirstOrDefault(y => y.Action == ProPresenterActions.CurrentSlide);

                    if (slide != null)
                    {
                        var text = new StringBuilder(slide.Text.Trim('\n'));

                        text.Replace('\n', ' ');

                        _logger.LogInfoFormat("Received {0}", text);

                        _vmixDa.SendText(text.ToString());
                    }
                    
                });

                BridgeOn = true;
            }
            else
            {
                cts.Cancel();
                _logger.LogInfo("Could not connect");
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

            _proPresenterDa.StopListen();
            await _proPresenterDa.Close();
            BridgeOn = false;
        }

        public void OnConnection(Action action)
        {
            OnConnectionAction = action;
        }
    }
}
