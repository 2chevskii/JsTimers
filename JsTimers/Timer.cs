using System;
using System.Threading;

namespace JsTimers
{
    public abstract class Timer
    {
        static volatile int lastId;

        readonly long                 _delay;
        readonly Action              _callback;
        readonly int                 _id;

        volatile bool             _isRef;
        ManualResetEvent _resetEvent;

        internal long nextExecutionTime = 0;
        internal bool _destroyed;

        public bool Destroyed
        {
            get => _destroyed;
            protected set
            {
                _destroyed = value;

                if (!value && _isRef)
                {
                    _resetEvent.Reset();
                }
                else
                {
                    _resetEvent.Set();
                }
            }
        }

        public static event Action<Timer, Exception> OnTimerError;
        public event Action<Exception> OnError;

        protected Timer(Action callback, int delay)
        {
            _delay = MillisecondsToTicks(delay);
            _callback = callback;
            _id = GetId();
            RefreshExecutionTime();
            _resetEvent = new ManualResetEvent(false); // FIXME

            AppDomain.CurrentDomain.ProcessExit += (_, __) => {
                _resetEvent.WaitOne();
            };
        }

        static int GetId()
        {
            lastId = lastId + 1;
            return lastId;
        }

        static long MillisecondsToTicks(int milliseconds)
        {
            return milliseconds * TimeSpan.TicksPerMillisecond;
        }

        public void Ref()
        {
            if (_destroyed)
            {
                return;
            }

            _isRef = true;
            _resetEvent.Reset();
        }

        public void UnRef()
        {
            if (_destroyed)
            {
                return;
            }

            _isRef = false;
            _resetEvent.Set();
        }

        public bool HasRef()
        {
            return _isRef;
        }

        protected void RefreshExecutionTime()
        {
            nextExecutionTime = TimerManager.TicksNow + _delay;
        }

        internal virtual void SafeExecute()
        {
            try
            {
                _callback.Invoke();
            }
            catch (Exception e)
            {
                bool shouldReport = true;
                if (OnError != null)
                {
                    OnError.Invoke(e);
                    shouldReport = false;
                }

                if (OnTimerError != null)
                {
                    OnTimerError.Invoke(this, e);
                    shouldReport = false;
                }

                if (shouldReport)
                {
                    Console.Error.WriteLine(BuildExceptionMessage(e));
                }
            }
        }

        string BuildExceptionMessage(Exception e)
        {
            return $"Timer#{_id} callback produced an unhandled exception:\n{e}";
        }
    }
}
