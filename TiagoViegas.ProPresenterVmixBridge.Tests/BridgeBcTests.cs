using System;
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
        public void BridgeBc_Bridge()
        {
        }
    }
}
