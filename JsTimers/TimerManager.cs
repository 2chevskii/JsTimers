using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JsTimers
{
    public static class TimerManager
    {
        static volatile int lastId;
        static volatile int refsCount;

        static Queue<Immediate> NextTickQueue = new Queue<Immediate>();

        static List<Timeout> ActiveTimeouts = new List<Timeout>();

        static AppDomain AppDomain;

        public static long TicksNow => DateTime.Now.Ticks;

        public static event Action<Timer, Exception> OnTimerError;

        static TimerManager()
        {
            AppDomain = AppDomain.CurrentDomain;
            AppDomain.ProcessExit += (_, __) =>
            {
                while (refsCount != 0)
                {
                    Thread.Sleep(1);
                }
            };

            Task.Run(
                () => {
                    while (true)
                    {
                        ProcessTick();
                        Thread.Sleep(2);
                    }
                }
            );
        }

        #region Public API

        public static Timeout SetTimeout(Action callback, int delay)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback), "Callback must be a function. Received null");
            }

            if (delay < 0)
            {
                DebugLogger.LogWarning("Creating timeout with delay < 0 is not possible. It will be set to 0");

                delay = 0;
            }

            var timeout = new Timeout(callback, delay, false);
            ActiveTimeouts.Add(timeout);
            return timeout;
        }

        public static Immediate SetImmediate(Action callback)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback), "Callback must be a function. Received null");
            }

            var immediate = new Immediate(callback);
            lock (NextTickQueue)
            {
                NextTickQueue.Enqueue(immediate);
            }
            return immediate;
        }

        public static Timeout SetInterval(Action callback, int interval)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback), "Callback must be a function. Received null");
            }

            var timeout = new Timeout(callback, interval, true);
            ActiveTimeouts.Add(timeout);
            return timeout;
        }

        public static void ClearInterval(Timeout interval)
        {
            ClearTimer(interval);
        }

        public static void ClearTimeout(Timeout timeout)
        {
            ClearTimer(timeout);
        }

        public static void ClearImmediate(Immediate immediate)
        {
            if (immediate is null || immediate.Destroyed)
            {
                return;
            }

            lock (NextTickQueue)
            {
                immediate.DestroyNow();
            }
        }

        #endregion

        static void ClearTimer(Timeout timer)
        {
            if (timer is null || timer.Destroyed)
            {
                return;
            }

            timer.DestroyNow();
        }

        internal static void RefMe(Timer timer)
        {
            lock (timer)
            {
                refsCount = refsCount + 1;

                DebugLogger.Log("Refed timer #{0}, total refs: {1}", timer.Id, refsCount);
            }
        }

        internal static void UnRefMe(Timer timer)
        {
            lock (timer)
            {
                refsCount = refsCount - 1;

                DebugLogger.Log("Unrefed timer #{0}, total refs: {1}", timer.Id, refsCount);
            }
        }

        internal static bool RaiseException(Timer timer, Exception exception)
        {
            if (OnTimerError == null)
            {
                return false;
            }

            OnTimerError.Invoke(timer, exception);
            return true;
        }

        internal static int GetId()
        {
            lastId = lastId + 1;
            return lastId;
        }

        internal static void Requeue(Timeout timeout)
        {
            ActiveTimeouts.Add(timeout);
        }

        static void ProcessTick()
        {
            lock (NextTickQueue)
            {
                while (NextTickQueue.Count > 0)
                {
                    Immediate immediate = NextTickQueue.Dequeue();
                    if (immediate.Destroyed)
                    {
                        continue;
                    }
                    immediate.SafeExecute();
                }
            }

            long ticksNow = TicksNow;
            for (int i = ActiveTimeouts.Count - 1; i >= 0; i--)
            {
                Timeout timeout = ActiveTimeouts[i];
                if (timeout.Destroyed)
                {
                    ActiveTimeouts.RemoveAt(i);
                    continue;
                }

                if (timeout.nextExecutionTime <= ticksNow)
                {
                    timeout.SafeExecute();
                }
            }
        }
    }
}
