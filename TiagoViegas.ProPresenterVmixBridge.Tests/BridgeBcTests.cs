using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TiagoViegas.ProPresenterVmixBridge.Business.Interfaces;

namespace TiagoViegas.ProPresenterVmixBridge.Tests
{
    [TestClass]
    public class BridgeBcTests
    {
        private readonly IBridgeBc _bridgeBc;

        public BridgeBcTests()
        {
            var container = ContainerHelper.CreateContainer();

            _bridgeBc = container.GetInstance<IBridgeBc>();
        }

        [TestMethod]
        public async Task BridgeBc_Bridge()
        {
            await _bridgeBc.Bridge();

            await Task.Delay(60000);

            await _bridgeBc.Close();
        }
    }
}
