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
        static Queue<Immediate> NextTickQueue = new Queue<Immediate>();

        static List<Timeout> ActiveTimeouts = new List<Timeout>();
        public static long TicksNow => DateTime.Now.Ticks;

        static TimerManager()
        {
            Task.Run(
                () => {
                    while (true)
                    {
                        ProcessTick();
                        Thread.Sleep(1);
                    }
                }
            );
        }

        public static Timeout SetTimeout(Action callback, int delay)
        {
            var timeout = new Timeout(callback, delay, false);
            ActiveTimeouts.Add(timeout);
            return timeout;
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
                    immediate.SafeExecute();
                }
            }

            long ticksNow = DateTime.Now.Ticks;
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
