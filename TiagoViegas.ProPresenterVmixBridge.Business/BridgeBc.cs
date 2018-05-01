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
        private readonly IConfigManager _configManager;

        public bool BridgeOn { get; set; }
        public bool Connecting { get; set; }
        private Action OnConnectionAction { get; set; }

        public BridgeBc(IProPresenterDataAgent proPresenterDataAgent, IVmixDataAgent vmixDataAgent, ILogger logger, IConfigManager configManager)
        {
            _vmixDa = vmixDataAgent;
            _logger = logger;
            _proPresenterDa = proPresenterDataAgent;
            _configManager = configManager;

            _proPresenterDa.OnDisconnected += OnProPresenterDisconnected;
            _proPresenterDa.OnProPresenterInstancesChanged += OnProPresenterFound;
            _proPresenterDa.OnConnected += OnProPresenterConnected;

            BridgeOn = false;
        }

        private void OnProPresenterConnected(object sender, EventArgs e)
        {
            OnConnectionAction?.Invoke();
            _logger.LogInfo("Connected");
            BridgeOn = true;

            _proPresenterDa.Listen((x) =>
            {
                var slide = x.Array.FirstOrDefault(y => y.Action == ProPresenterActions.CurrentSlide);

                if (slide != null)
                {
                    var text = new StringBuilder(slide.Text.Trim('\n'));

                    text.Replace("\r\n", " ");
                    text.Replace('\n', ' ');

                    _logger.LogInfoFormat("Received {0}", text);

                    try
                    {
                        _vmixDa.SendText(text.ToString());

                    }catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                }

            });
        }

        private async void OnProPresenterFound(object sender, ProPresenterInstancesChangedEventArgs args)
        {
            var instances = args.Instances;

            if (instances.Any() && !BridgeOn)
            {
                var cts = new CancellationTokenSource();

                if (instances.Count() == 1)
                {
                    await _proPresenterDa.Connect(instances.First().Name, cts.Token);
                }
                else
                {
                    var configInstanceName = _configManager.GetConfig(ConfigKeys.ProPresenterName);

                    var instance = instances.FirstOrDefault(x => x.Name == configInstanceName);

                    if(instance != null)
                    {
                        await _proPresenterDa.Connect(configInstanceName, cts.Token);
                    }
                }

                if (!_proPresenterDa.Connected)
                {
                    cts.Cancel();
                    _logger.LogInfo("Could not connect");
                    BridgeOn = false;
                }
            }
        }


        private void OnProPresenterDisconnected(object sender, EventArgs args)
        {
            BridgeOn = false;
        }


        public void Bridge()
        {
            _proPresenterDa.LookForProPresenter();
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
