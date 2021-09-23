using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsTimers.Tests
{
    [TestClass]
    public class TimerManagerTest
    {
        [TestMethod]
        public void SetTimeoutTest()
        {
            var testStartTime = DateTime.Now;
            var testEndTime = default(DateTime);

            var timeout = TimerManager.SetTimeout(
                () =>
                {
                    testEndTime = DateTime.Now;
                },
                1000
            );

            Thread.Sleep(1500);

            Assert.IsTrue(timeout.Destroyed);
            var msPassed = (testEndTime - testStartTime).TotalMilliseconds;

            Assert.IsTrue(msPassed >= 1000 && msPassed <= 1500);
        }

        [TestMethod]
        public void SetIntervalTest()
        {
            var testStartTime = DateTime.Now;
            var testEndTime = default(DateTime);
            var callbacksExecuted = 0;

            var interval = TimerManager.SetInterval(
                () =>
                {
                    callbacksExecuted++;

                    if (callbacksExecuted == 3)
                    {
                        testEndTime = DateTime.Now;
                    }
                },
                300
            );
            interval.UnRef();

            Thread.Sleep(1000);
            var msPassed = (testEndTime - testStartTime).TotalMilliseconds;

            Assert.IsFalse(interval.Destroyed);
            Assert.IsFalse(interval.HasRef());
            Assert.IsTrue(msPassed >= 900 && msPassed <= 1000);
        }

        [TestMethod]
        public void SetImmediateTest()
        {
            bool fired = false;

            TimerManager.SetImmediate(
                () =>
                {
                    fired = true;
                }
            );

            Thread.Sleep(50);
            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ClearTimeoutTest()
        {
            bool fired = false;
            var timeout = TimerManager.SetTimeout(() => fired = true, 0);

            TimerManager.ClearTimeout(timeout);
            Thread.Sleep(50);
            Assert.IsFalse(fired);
        }

        [TestMethod]
        public void ClearIntervalTest()
        {
            bool fired = false;
            var timeout = TimerManager.SetInterval(() => fired = true, 0);

            TimerManager.ClearInterval(timeout);
            Thread.Sleep(50);
            Assert.IsFalse(fired);
        }

        [TestMethod]
        public void ClearImmediateTest()
        {
            bool fired = false;
            var timeout = TimerManager.SetImmediate(() => fired = true);

            TimerManager.ClearImmediate(timeout);
            Thread.Sleep(50);
            Assert.IsFalse(fired);
        }
    }
}
