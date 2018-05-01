using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArkaneSystems.Arkane.Zeroconf;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Entities;
using TiagoViegas.ProPresenterVmixBridge.Logging;

namespace TiagoViegas.ProPresenterVmixBridge.DataAgents
{
    public class ProPresenterDataAgent : IProPresenterDataAgent
    {
        private readonly string _password;
        private ClientWebSocket _socket;
        public bool Connected { get; set; }
        public bool Connecting { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private readonly ILogger _logger;
        private readonly ICollection<ProPresenterInstance> _instances;
        private string ConnectedInstance { get; set; }
        public IReadOnlyCollection<ProPresenterInstance> Instances => (IReadOnlyCollection<ProPresenterInstance>)_instances;

        public event EventHandler<ProPresenterInstancesChangedEventArgs> OnProPresenterInstancesChanged;
        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;

        private readonly ServiceBrowser _serviceBrowser;

        private bool StopListening { get; set; }

        public ProPresenterDataAgent(IConfigManager configManager, ILogger logger)
        {
            _instances = new List<ProPresenterInstance>();

            _serviceBrowser = new ServiceBrowser();

            _serviceBrowser.ServiceAdded += OnServiceAdded;
            _serviceBrowser.ServiceRemoved += OnServiceRemoved;

            _logger = logger;
            try
            {
                _password = configManager.GetConfig(ConfigKeys.ProPresenterPassword);
            }
            catch (Exception e)
            {
                _password = "";
                _logger.LogError("Error reading configuration file", e);

                configManager.EditConfig(ConfigKeys.ProPresenterPassword, _password);
                configManager.SaveConfig();
            }
            
            Connected = false;
            Connecting = false;
            StopListening = false;
        }

        private async void OnServiceRemoved(object o, ServiceBrowseEventArgs args)
        {
            var name = args.Service.Name;

            var instance = _instances.FirstOrDefault(x => x.Name == name);

            if (instance != null)
            {
                _instances.Remove(instance);

                if (Connected && ConnectedInstance == instance.Name)
                {
                    StopListen();
                    await Close();
                    ConnectedInstance = null;
                }

                OnProPresenterInstancesChanged?.Invoke(this, new ProPresenterInstancesChangedEventArgs { Instances = _instances.ToList() });
            }
        }

        private void OnServiceAdded(object o, ServiceBrowseEventArgs args)
        {
            args.Service.Resolved += OnResolved;
            args.Service.Resolve();
        }

        private void OnResolved(object o, ServiceResolvedEventArgs args)
        {
            if (_instances.All(x => x.Name != args.Service.Name))
            {
                _instances.Add(new ProPresenterInstance
                {
                    Name = args.Service.Name,
                    Port = ((ushort)args.Service.Port).ToString(),
                    Ip = args.Service.HostEntry.AddressList[0].ToString(),
                });

                OnProPresenterInstancesChanged?.Invoke(this, new ProPresenterInstancesChangedEventArgs { Instances = _instances.ToList()});
            }
        }

        public void LookForProPresenter()
        {
            _serviceBrowser.Browse("_pro6stagedsply._tcp", "local");
        }

        public async Task Connect(string instanceName, CancellationToken cancellationToken)
        {
            var instance = _instances.FirstOrDefault(x => x.Name == instanceName);

            if (instance != null)
            {
                await Connect(cancellationToken, instance.Ip, instance.Port);
                ConnectedInstance = instance.Name;
            }
            else
            {
                _logger.LogErrorFormat("No instance found with name: {0}", instanceName);
            }
        }


        private async Task Connect(CancellationToken cancellationToken, string ip, string port)
        {
            if (Connecting)
            {
                return;
            }

            _socket = new ClientWebSocket();

            _socket.Options.KeepAliveInterval = new TimeSpan(24,0,0);

            Connecting = true;

            try
            {   
                _logger.LogInfoFormat("Connecting to {0}", $"ws://{ip}:{port}/stagedisplay");
                await _socket.ConnectAsync(new Uri($"ws://{ip}:{port}/stagedisplay"), cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError("Could not connect", e);
                Connected = false;
                Connecting = false;
                return;
            }
            

            var authCommand = JsonConvert.SerializeObject(new
            {
                pwd = _password,
                ptl = 610,
                acn = "ath"
            });

            var receiveTask = new Task(async () =>
            {
                var rcvArray = new byte[2048];
                var rcvBuffer = new ArraySegment<byte>(rcvArray);

                while (true)
                {
                    if(_socket.State == WebSocketState.Open)
                    {
                        var result = await _socket.ReceiveAsync(rcvBuffer, cancellationToken);

                        var resultArray = rcvBuffer.Skip(rcvBuffer.Offset).Take(result.Count).ToArray();

                        var message = Encoding.UTF8.GetString(resultArray);

                        var messageObject = JsonConvert.DeserializeObject<ProPresenterMessage>(message);

                        if (messageObject != null && messageObject.Action == ProPresenterActions.Auth)
                        {
                            var authMessage = JsonConvert.DeserializeObject<ProPresenterAuthMessage>(message);

                            if (authMessage.Authorized)
                            {
                                Connected = true;
                                _logger.LogInfo("Connected");
                                OnConnected?.Invoke(this, new EventArgs());
                            }
                            break;
                        }
                    }
                    else
                    {
                        Connected = false;
                        break;
                    }
                }
                
                Connecting = false;
            }, cancellationToken);


            receiveTask.Start();

            await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(authCommand)), WebSocketMessageType.Text, true, cancellationToken);

            while (Connecting);
        }

        public void Listen(Action<ProPresenterNewSlideMessage> action)
        {
            CancellationTokenSource = new CancellationTokenSource();

            StopListening = false;

            Task.Factory.StartNew(async () =>
            {
                var rcvArray = new byte[2048];
                var rcvBuffer = new ArraySegment<byte>(rcvArray);

                while (!StopListening)
                {
                    var result = await _socket.ReceiveAsync(rcvBuffer, CancellationTokenSource.Token);

                    var resultArray = rcvBuffer.Skip(rcvBuffer.Offset).Take(result.Count).ToArray();

                    var message = Encoding.UTF8.GetString(resultArray);

                    var messageObject = JsonConvert.DeserializeObject<ProPresenterMessage>(message);

                    if (messageObject != null && messageObject.Action == ProPresenterActions.NewSlide)
                    {
                        var newSlide = JsonConvert.DeserializeObject<ProPresenterNewSlideMessage>(message);
                        action(newSlide);
                    }
                }

                StopListening = false;
                
            }, CancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void StopListen()
        {
            StopListening = true;
        }

        public async Task Close()
        {
            if (Connecting)
            {
                return;
            }

            CancellationTokenSource = new CancellationTokenSource();
            try
            {
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationTokenSource.Token);
            }
            catch (Exception)
            {
            }
            _socket.Dispose();
            Connected = false;
            Connecting = false;
            _logger.LogInfo("Disconnected");
            OnDisconnected?.Invoke(this, new EventArgs());
        }
    }
}
