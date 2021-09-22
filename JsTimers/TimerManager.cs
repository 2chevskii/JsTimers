using System;
using System.Collections.Generic;
using System.Timers;

namespace JsTimers
{
    public static class TimerManager
    {
        readonly static object              sync = new object();
        static          System.Timers.Timer Timer;

        static long elapsedTicks;

        static Queue<Immediate>     NextTickQueue = new Queue<Immediate>();
        static List<Timeout>        Timeouts      = new List<Timeout>();

        public static event Action<Exception> OnError;

        static TimerManager()
        {
            Timer = new System.Timers.Timer();
            Timer.Interval = 1d;
            Timer.AutoReset = true;
            Timer.Elapsed += OnTick;
        }

        public static Immediate SetImmediate(Action callback)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback), "Callback must be a function. Received null");
            }

            var immediate = new Immediate(callback);
            RunImmediate(immediate);

            return immediate;
        }

        public static Timeout SetTimeout(Action callback, int timeout)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback), "Callback must be a function. Received null");
            }

            var timer = new Timeout(callback, timeout, false);
            RunTimeout(timer);

            return timer;
        }

        public static Timeout SetInterval(Action callback, int interval)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback), "Callback must be a function. Received null");
            }

            var timer = new Timeout(callback, interval, true);
            RunTimeout(timer);

            return timer;
        }

        public static void ClearImmediate(Immediate immediate)
        {
            immediate.Destroy(false);

        }

        internal static void RunImmediate(Immediate immediate)
        {
            NextTickQueue.Enqueue(immediate);
            OnQueueUpdated();
        }

        internal static void RunTimeout(Timeout timeout)
        {
            Timeouts.Add(timeout);
            OnQueueUpdated();
        }

        static void OnQueueUpdated()
        {
            lock (sync)
            {
                if (!Timer.Enabled)
                {
                    StartTicking();
                }
            }
        }

        static void SafeExecute(Timer timer)
        {
            try
            {
                timer.Execute();
            }
            catch (Exception e)
            {
                if (OnError != null)
                {
                    OnError.Invoke(e);
                }
                else
                {
                    Console.Error.WriteLine(e);
                }
            }
        }

        static void OnTick(object sender, ElapsedEventArgs args)
        {
            elapsedTicks++;
            while (NextTickQueue.Count > 0)
            {
                Immediate immediate = NextTickQueue.Dequeue();
                if(!immediate.Destroyed)
                    SafeExecute(immediate);
            }

            for (int i = Timeouts.Count - 1; i >= 0; i--)
            {
                Timeout timeout = Timeouts[i];
                int duration = timeout.DelayMilliseconds;
                if (elapsedTicks % duration != 0)
                {
                    continue;
                }

                SafeExecute(timeout);

                if (timeout.Destroyed)
                {
                    Timeouts.RemoveAt(i);
                }
            }

            lock (sync)
            {
                if (Timeouts.Count == 0)
                {
                    StopTicking();
                }
            }
        }

        static void StartTicking()
        {
            elapsedTicks = 0;
            Timer.Enabled = true;
        }

        static void StopTicking()
        {
            Timer.Enabled = false;
        }
    }
}
