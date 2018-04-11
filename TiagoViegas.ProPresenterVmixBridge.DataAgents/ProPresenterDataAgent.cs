using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Entities;
using TiagoViegas.ProPresenterVmixBridge.Logging;

namespace TiagoViegas.ProPresenterVmixBridge.DataAgents
{
    public class ProPresenterDataAgent : IProPresenterDataAgent
    {
        private readonly string _ip;
        private readonly string _port;
        private readonly string _password;
        private ClientWebSocket _socket;
        public bool Connected { get; set; }
        public bool Connecting { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private readonly ILogger _logger;

        private bool StopListening { get; set; }

        public ProPresenterDataAgent(IConfigManager configManager, ILogger logger)
        {
            _logger = logger;
            try
            {
                _ip = configManager.GetConfig(ConfigKeys.ProPresenterIp);
                _port = configManager.GetConfig(ConfigKeys.ProPresenterPort);
                _password = configManager.GetConfig(ConfigKeys.ProPresenterPassword);
            }
            catch (Exception e)
            {
                _ip = "127.0.0.1";
                _port = "50001";
                _password = "";
                _logger.LogError("Error reading configuration file", e);

                configManager.EditConfig(ConfigKeys.ProPresenterIp, _ip);
                configManager.EditConfig(ConfigKeys.ProPresenterPort, _port);
                configManager.EditConfig(ConfigKeys.ProPresenterPassword, _password);

                configManager.SaveConfig();
            }
            
            Connected = false;
            Connecting = false;
            StopListening = false;
        }

        public async Task Connect(CancellationToken cancellationToken)
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
                _logger.LogInfoFormat("Connecting to {0}", $"ws://{_ip}:{_port}/stagedisplay");
                await _socket.ConnectAsync(new Uri($"ws://{_ip}:{_port}/stagedisplay"), cancellationToken);
            }
            catch (Exception e)
            {
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
                var rcvArray = new byte[128];
                var rcvBuffer = new ArraySegment<byte>(rcvArray);

                while (true)
                {
                    if(_socket.State == WebSocketState.Open)
                    {
                        var result = await _socket.ReceiveAsync(rcvBuffer, cancellationToken);

                        var resultArray = rcvBuffer.Skip(rcvBuffer.Offset).Take(result.Count).ToArray();

                        var message = Encoding.UTF8.GetString(resultArray);

                        var messageObject = JsonConvert.DeserializeObject<ProPresenterAuthMessage>(message);

                        if (messageObject != null && messageObject.Action == ProPresenterActions.Auth)
                        {
                            if (messageObject.Authorized)
                            {
                                Connected = true;
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
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationTokenSource.Token);
            _socket.Dispose();
            Connected = false;
            Connecting = false;
        }
    }
}
