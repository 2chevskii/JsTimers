using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static JsTimers.TimerManager;
using static JsTimers.Tests.Helpers;

namespace JsTimers.Tests
{
    [TestClass]
    public class TimerManagerTest
    {
        [TestMethod]
        public void SetTimeoutTest()
        {
            Timeout timeout = SetTimeout(Noop, ZERO);
            Assert.IsNotNull(timeout);
            Assert.IsFalse(timeout.Destroyed);
            Assert.IsFalse(timeout.IsInterval);
            Assert.IsTrue(timeout.HasRef());
        }

        [TestMethod]
        public void SetIntervalTest()
        {
            Timeout timeout = SetInterval(Noop, ZERO);
            Assert.IsNotNull(timeout);
            Assert.IsFalse(timeout.Destroyed);
            Assert.IsTrue(timeout.IsInterval);
            Assert.IsTrue(timeout.HasRef());
        }

        [TestMethod]
        public void SetImmediateTest()
        {
            Immediate immediate = SetImmediate(Noop);
            Assert.IsNotNull(immediate);
            Assert.IsFalse(immediate.Destroyed);
            Assert.IsTrue(immediate.HasRef());
        }

        [DataTestMethod]
        [DataRow(ZERO, 100)]
        [DataRow(5, 100)]
        [DataRow(10, 100)]
        [DataRow(40, 100)]
        [DataRow(80, 100)]
        [DataRow(100, 120)]
        [DataRow(150, 200)]
        public void SetTimeoutExecutionTest(int delay, int sleepTime)
        {
            int timesFired = 0;

            Timeout timeout = SetTimeout(() => timesFired++, delay);

            Thread.Sleep(sleepTime);

            Assert.IsTrue(timeout.Destroyed);
            Assert.AreEqual(1, timesFired);
        }

        // [TestMethod] // BUG: Unreliable, temporarily disabled
        // public void SetIntervalExecutionTest()
        // {
        //     int timesFired = 0;

        //     Timeout interval = SetInterval(
        //         () => {
        //             if(timesFired < 3)
        //             {
        //                 timesFired++;
        //             }
        //         }, 10);

        //     Thread.Sleep(50);

        //     Assert.IsFalse(interval.Destroyed);
        //     Assert.AreEqual(3, timesFired);
        // }

        [TestMethod]
        public void SetImmediateExecutionTest()
        {
            int timesFired = 0;

            Immediate immediate = SetImmediate(() => timesFired++);

            Thread.Sleep(30);

            Assert.IsTrue(immediate.Destroyed);
            Assert.AreEqual(1, timesFired);
        }

        [TestMethod]
        public void ClearTimeoutTest()
        {
            int timesFired = 0;

            var timeout = SetTimeout(Noop, ZERO);

            ClearTimeout(timeout);

            Assert.IsTrue(timeout.Destroyed);
            Assert.AreEqual(0, timesFired);
        }

        [TestMethod]
        public void ClearIntervalTest()
        {
            int timesFired = 0;

            var interval = SetInterval(Noop, ZERO);

            ClearInterval(interval);

            Assert.IsTrue(interval.Destroyed);
            Assert.AreEqual(0, timesFired);
        }

        [TestMethod]
        public void ClearImmediateTest()
        {
            int timesFired = 0;

            var immediate = SetImmediate(Noop);

            ClearImmediate(immediate);

            Assert.IsTrue(immediate.Destroyed);
            Assert.AreEqual(0, timesFired);
        }

        [TestMethod]
        public void ClearTimeoutWithClearIntervalTest()
        {
            int timesFired = 0;

            var timeout = SetTimeout(Noop, ZERO);

            ClearInterval(timeout);

            Assert.IsTrue(timeout.Destroyed);
            Assert.AreEqual(0, timesFired);
        }

        [TestMethod]
        public void ClearIntervalWithClearTimeoutTest()
        {
            int timesFired = 0;

            var interval = SetInterval(Noop, ZERO);

            ClearTimeout(interval);

            Assert.IsTrue(interval.Destroyed);
            Assert.AreEqual(0, timesFired);
        }
    }
}
