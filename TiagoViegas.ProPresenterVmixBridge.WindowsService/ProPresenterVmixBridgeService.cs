using System;
using System.Diagnostics;
using System.ServiceProcess;
using TiagoViegas.ProPresenterVmixBridge.Business.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.WindowsService.IoC;

namespace TiagoViegas.ProPresenterVmixBridge.WindowsService
{
    public partial class ProPresenterVmixBridgeService : ServiceBase
    {
        private readonly IBridgeBc _bridgeBc;

        public ProPresenterVmixBridgeService()
        {
            InitializeComponent();

            var container = IoCManager.CreateContainer();
            _bridgeBc = container.GetInstance<IBridgeBc>();
           
            
        }

        protected override void OnStart(string[] args)
        {
            _bridgeBc.Bridge();
             
        }

        protected override void OnStop()
        {
            _bridgeBc.Close();
        }
    }
}
