using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Entities;
using WebSocketSharp;

namespace TiagoViegas.ProPresenterVmixBridge.DataAgents
{
    public class ProPresenterDataAgent : IProPresenterDataAgent
    {
        private readonly string _ip;
        private readonly string _port;
        private readonly string _password;
        private WebSocket _socket;
        public bool Connected { get; set; }
        public bool Connecting { get; set; }

        public ProPresenterDataAgent(IConfigManager configManager)
        {
            _ip = configManager.GetConfig(ConfigKeys.ProPresenterIp);
            _port = configManager.GetConfig(ConfigKeys.ProPresenterPort);
            _password = configManager.GetConfig(ConfigKeys.ProPresenterPassword);
            Connected = false;
        }

        public void Connect()
        {
            _socket = new WebSocket($"ws://{_ip}:{_port}/stagedisplay");

            var authCommand = JsonConvert.SerializeObject(new
            {
                pwd = _password,
                ptl = 610,
                acn = "ath"
            });

            _socket.OnMessage += (sender, e) => {
                Connected = true;
                Connecting = false;
            };

            _socket.OnError += (sender, e) =>
            {
                Console.WriteLine("Error connecting");
                Connected = false;
                _socket.Close();
                Connecting = false;
            };

            _socket.Connect();
            Connecting = true;
            _socket.Send(authCommand);
        }

        public void Listen(EventHandler<MessageEventArgs> onMessage)
        {
            _socket.OnMessage += onMessage;
        }

        public void Close()
        {
            _socket.Close();
            Connected = false;
        }
    }
}
