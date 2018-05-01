using System.ServiceProcess;
using TiagoViegas.ProPresenterVmixBridge.Business.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Logging;

namespace TiagoViegas.ProPresenterVmixBridge.WindowsService
{
    public partial class ProPresenterVmixBridgeService : ServiceBase
    {
        private readonly IBridgeBc _bridgeBc;
        private readonly ILogger _logger;


        public ProPresenterVmixBridgeService(IBridgeBc bridgeBc, ILogger logger)
        {
            InitializeComponent();

            _bridgeBc = bridgeBc;
            _logger = logger;
        }

        protected override void OnStart(string[] args)
        {
            _logger.LogInfo("Starting service");

            _bridgeBc.Bridge();

            _logger.LogInfo("Service started");
        }

        protected override void OnStop()
        {
            _logger.LogInfo("Stoping service");
            _bridgeBc.Close();
            _logger.LogInfo("Service stopped");
        }
    }
}