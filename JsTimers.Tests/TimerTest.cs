using Microsoft.VisualStudio.TestTools.UnitTesting;

using static JsTimers.TimerManager;
using static JsTimers.Tests.Helpers;

namespace JsTimers.Tests
{
    [TestClass]
    public class TimerTest
    {
        [TestMethod]
        public void SetRefEnableDisableTestTimeout()
        {
            Timeout timeout = SetTimeout(Noop, ZERO);

            Assert.IsTrue(timeout.HasRef());

            timeout.UnRef();
            Assert.IsFalse(timeout.HasRef());

            timeout.Ref();
            Assert.IsTrue(timeout.HasRef());
        }

        [TestMethod]
        public void SetRefEnableDisableTestInterval()
        {
            Timeout interval = SetTimeout(Noop, ZERO);

            Assert.IsTrue(interval.HasRef());

            interval.UnRef();
            Assert.IsFalse(interval.HasRef());

            interval.Ref();
            Assert.IsTrue(interval.HasRef());
        }

        [TestMethod]
        public void SetRefEnableDisableTestImmediate()
        {
            Immediate immediate = SetImmediate(Noop);

            Assert.IsTrue(immediate.HasRef());

            immediate.UnRef();
            Assert.IsFalse(immediate.HasRef());

            immediate.Ref();
            Assert.IsTrue(immediate.HasRef());
        }
    }
}
