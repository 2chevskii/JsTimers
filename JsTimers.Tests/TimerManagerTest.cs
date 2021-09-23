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
    }
}
