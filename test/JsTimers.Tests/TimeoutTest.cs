using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static JsTimers.TimerManager;
using static JsTimers.Tests.Helpers;

namespace JsTimers.Tests
{
    [TestClass]
    public class TimeoutTest
    {
        [TestMethod]
        public void RefreshTest()
        {
            int timesFired = 0;

            var timeout = SetTimeout(() => timesFired++, ZERO);

            Thread.Sleep(100);

            Assert.AreEqual(1, timesFired);
            Assert.IsTrue(timeout.Destroyed);

            timeout.Refresh();

            Assert.IsFalse(timeout.Destroyed);

            Thread.Sleep(100);

            Assert.AreEqual(2, timesFired);
            Assert.IsTrue(timeout.Destroyed);
        }
    }
}
