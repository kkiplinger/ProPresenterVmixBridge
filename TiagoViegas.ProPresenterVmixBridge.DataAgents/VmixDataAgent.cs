using System;
using System.Net.Http;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Entities;
using TiagoViegas.ProPresenterVmixBridge.Logging;

namespace TiagoViegas.ProPresenterVmixBridge.DataAgents
{
    public class VmixDataAgent : IVmixDataAgent
    {
        private readonly string _inputNumber;
        private readonly Uri _baseAddress;

        public VmixDataAgent(IConfigManager configManager, ILogger logger)
        {
            try
            {
                var ip = configManager.GetConfig(ConfigKeys.VmixIp);
                var port = configManager.GetConfig(ConfigKeys.VmixPort);
                _inputNumber = configManager.GetConfig(ConfigKeys.VmixInputNumber);
                _baseAddress = new Uri($"http://{ip}:{port}");
            }
            catch (Exception e)
            {
                logger.LogError("Error while reading configuration file", e);

                const string ip = "127.0.0.1";
                const string port = "8088";
                _inputNumber = "1";
                _baseAddress = new Uri($"http://{ip}:{port}");


                configManager.EditConfig(ConfigKeys.VmixIp, ip);
                configManager.EditConfig(ConfigKeys.VmixPort, port);
                configManager.EditConfig(ConfigKeys.VmixInputNumber, _inputNumber);
            }
        }

        public async Task SendText(string text) {
            using(var client = new HttpClient())
            {
                client.BaseAddress = _baseAddress;

                var result = await client.GetAsync($"/api/?Function=SetText&SelectedIndex=0&Input={_inputNumber}&Value={text}");

                result.EnsureSuccessStatusCode();
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
