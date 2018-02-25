using System;
using System.Net.Http;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Entities;

namespace TiagoViegas.ProPresenterVmixBridge.DataAgents
{
    public class VmixDataAgent : IVmixDataAgent
    {
        private readonly string _ip;
        private readonly string _port;
        private readonly string _inputNumber;
        private readonly System.Uri _baseAddress;

        public VmixDataAgent(IConfigManager configManager)
        {
            _ip = configManager.GetConfig(ConfigKeys.VmixIp);
            _port = configManager.GetConfig(ConfigKeys.VmixPort);
            _inputNumber = configManager.GetConfig(ConfigKeys.VmixInputNumber);

            _baseAddress = new Uri($"http://{_ip}:{_port}");
        }

        public async Task SendText(string text) {
            using(var client = new HttpClient())
            {
                client.BaseAddress = _baseAddress;

                var result = await client.GetAsync($"/api/?Function=SetText&SelectedIndex=0&Input={_inputNumber}&Value={text}");

                result.EnsureSuccessStatusCode();

                Console.WriteLine(_baseAddress + $"/api/?Function=SetText&SelectedIndex=0&Input={_inputNumber}&Value={text}");
            }
        }

        public async Task<bool> CheckConfig()
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = _baseAddress;

                try
                {
                    var result = await client.GetAsync("/api");

                    return result.IsSuccessStatusCode;
                }catch(Exception)
                {
                    return false;
                }
                
            }
        }
    }
}
