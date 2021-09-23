using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            AppDomain.ProcessExit += (_, __) => {
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
            return SetTimeout(callback, ConvertToMilliseconds(delay));
        }

        public static Timeout SetTimeout(object obj, string methodName, int delay, params object[] args)
        {
            var callback = GetCallbackFromInstanceMethod(obj, methodName, args);

            return SetTimeout(callback, delay);
        }

        public static Timeout SetTimeout(Type type, string methodName, int delay, params object[] args)
        {
            var callback = GetCallbackFromStaticMethod(type, methodName, args);

            return SetTimeout(callback, delay);
        }

        public static Timeout SetTimeout(object obj, string methodName, float delay, params object[] args)
        {
            return SetTimeout(obj, methodName, ConvertToMilliseconds(delay), args);
        }

        public static Timeout SetTimeout(Type type, string methodName, float delay, params object[] args)
        {
            return SetTimeout(type, methodName, ConvertToMilliseconds(delay), args);
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
            return SetInterval(callback, ConvertToMilliseconds(interval));
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

        public static Immediate SetImmediate(object obj, string methodName, params object[] args)
        {
            var callback = GetCallbackFromInstanceMethod(obj, methodName, args);

            return SetImmediate(callback);
        }

        public static Immediate SetImmediate(Type type, string methodName, params object[] args)
        {
            var callback = GetCallbackFromStaticMethod(type, methodName, args);

            return SetImmediate(callback);
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

        static Action GetCallbackFromStaticMethod(Type type, string methodName, object[] args)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type), "Target object cannot be null");
            }

            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentException($"'{methodName ?? "null"}' is not a valid method identifier");
            }

            var methodInfo = type.GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (methodInfo is null)
            {
                throw new InvalidOperationException($"Method '{methodName}' was not found in type {type.Name}");
            }

            return BuildCallback(methodInfo, args);
        }

        static Action GetCallbackFromInstanceMethod(object obj, string methodName, object[] args)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj), "Target object cannot be null");
            }

            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentException($"'{methodName ?? "null"}' is not a valid method identifier");
            }

            var type = obj.GetType();

            var methodInfo = type.GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (methodInfo is null)
            {
                throw new InvalidOperationException($"Method '{methodName}' was not found in type {type.Name}");
            }

            var aargs = new object[args.Length + 1];
            aargs[0] = obj;
            args.CopyTo(aargs, 1);

            return BuildCallback(methodInfo, aargs);
        }

        static Action BuildCallback(MethodInfo methodInfo, params object[] args)
        {
            if (methodInfo.IsStatic)
            {
                return () => {
                    methodInfo.Invoke(null, args);
                };
            }

            if (args.Length == 1)
            {
                return () => {
                    methodInfo.Invoke(args[0], Array.Empty<object>());
                };
            }

            var obj = args[0];
            var remainingArgs = new object[args.Length - 1];

            for (var i = 1; i < args.Length; i++)
            {
                remainingArgs[i - 1] = args[i];
            }

            return () => {
                methodInfo.Invoke(obj, remainingArgs);
            };
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

        static int ConvertToMilliseconds(float seconds)
        {
            return (int)(seconds * 1000);
        }
    }
}
