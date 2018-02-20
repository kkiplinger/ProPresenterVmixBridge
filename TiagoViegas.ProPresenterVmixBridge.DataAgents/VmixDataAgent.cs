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

        public VmixDataAgent(IConfigManager configManager)
        {
            _ip = configManager.GetConfig(ConfigKeys.VmixIp);
            _port = configManager.GetConfig(ConfigKeys.VmixPort);
            _inputNumber = configManager.GetConfig(ConfigKeys.VmixInputNumber);
        }

        public async Task SendText(string text) {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri($"http://{_ip}:{_port}/api/");

                await client.GetAsync($"?Function=SetText&SelectedIndex=0&Input={_inputNumber}&Value={text}");
            }
        }
    }
}
