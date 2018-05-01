using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TiagoViegas.ProPresenterVmixBridge.Business.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;

namespace TiagoViegas.ProPresenterVmixBridge.Tests
{
    [TestClass]
    public class BridgeBcTests
    {
        private readonly IBridgeBc _bridgeBc;
        private readonly IProPresenterDataAgent _presenterDataAgent;

        public BridgeBcTests()
        {
            var container = ContainerHelper.CreateContainer();

            _bridgeBc = container.GetInstance<IBridgeBc>();
            _presenterDataAgent = container.GetInstance<IProPresenterDataAgent>();
        }

        [TestMethod]
        public async Task BridgeBc_Bridge()
        {
            _bridgeBc.Bridge();

            await Task.Delay(60000);

            await _bridgeBc.Close();
        }


        [TestMethod]
        public async Task ProPresenterDataAgent_Look()
        {
             _presenterDataAgent.LookForProPresenter();

            while (!_presenterDataAgent.Instances.Any()) { }

            var instances = _presenterDataAgent.Instances;
        }
    }
}
