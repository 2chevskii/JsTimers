using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JsTimers
{
    /// <summary>
    /// Contains essential methods for managing timers
    /// </summary>
    public static class TimerManager
    {
        static volatile int              lastId;
        static volatile int              refsCount;
        static volatile bool             isShuttingDown;
        static          Queue<Immediate> NextTickQueue  = new Queue<Immediate>();
        static          List<Timeout>    ActiveTimeouts = new List<Timeout>();

        internal static long TicksNow => DateTime.Now.Ticks;

        /// <summary>
        /// Intercepts exceptions thrown in all timers' callbacks. Suppresses stderr throw if at least one subscriber is present
        /// </summary>
        public static event Action<Timer, Exception> OnTimerError;

        static TimerManager()
        {
            AppDomain.CurrentDomain.ProcessExit += (_, __) => {
                while (refsCount != 0)
                {
                    Thread.Sleep(1);
                }

                isShuttingDown = true;
            };

            Task.Run(
                () => {
                    while (!isShuttingDown)
                    {
                        ProcessTick();
                        Thread.Sleep(2);
                    }
                }
            );
        }

        #region Public API

        /// <summary>
        /// Sets a timer which executes <paramref name="callback"/> after <paramref name="delay"/> milliseconds
        /// </summary>
        /// <param name="callback">Action to execute after <paramref name="delay"/> has passed</param>
        /// <param name="delay">Delay in milliseconds</param>
        /// <returns></returns>
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

        /// <summary>
        /// Sets a timer which executes <paramref name="callback"/> after <paramref name="delay"/> seconds
        /// </summary>
        /// <param name="callback">Action to execute after <paramref name="delay"/> has passed</param>
        /// <param name="delay">Delay in seconds</param>
        /// <returns></returns>
        public static Timeout SetTimeout(Action callback, float delay)
        {
            return SetTimeout(callback, Utility.SecondsToMilliseconds(delay));
        }

        /// <summary>
        /// Sets timer which repeatedly executes <paramref name="callback"/> every <paramref name="interval"/> milliseconds
        /// </summary>
        /// <param name="callback">Action to execute every time <paramref name="interval"/> has passed</param>
        /// <param name="interval">Interval in milliseconds</param>
        /// <returns></returns>
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

        /// <summary>
        /// Sets timer which repeatedly executes <paramref name="callback"/> every <paramref name="interval"/> seconds
        /// </summary>
        /// <param name="callback">Action to execute every time <paramref name="interval"/> has passed</param>
        /// <param name="interval">Interval in seconds</param>
        /// <returns></returns>
        public static Timeout SetInterval(Action callback, float interval)
        {
            return SetInterval(callback, Utility.SecondsToMilliseconds(interval));
        }

        /// <summary>
        /// Sets action to execute on next <see cref="TimerManager"/> tick
        /// </summary>
        /// <param name="callback">Action to execute</param>
        /// <returns></returns>
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

        /// <summary>
        /// Cancels specified timeout. Has no effect on timers which were already destroyed
        /// </summary>
        /// <param name="timeout">Timer to cancel</param>
        public static void ClearTimeout(Timeout timeout)
        {
            ClearTimer(timeout);
        }

        /// <summary>
        /// Cancels subsequent executions of specified interval timer. Has no effect on timers which were already destroyed
        /// </summary>
        /// <param name="interval">Timer to cancel</param>
        public static void ClearInterval(Timeout interval)
        {
            ClearTimer(interval);
        }

        /// <summary>
        /// Cancels execution of given immediate. Has no effect on immediates which were already executed/cancelled
        /// </summary>
        /// <param name="immediate">Immediate to cancel</param>
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

        static void ClearTimer(Timeout timer)
        {
            if (timer is null || timer.Destroyed)
            {
                return;
            }

            timer.DestroyNow();
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
