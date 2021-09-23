using System;
using System.Diagnostics;
using System.Threading;

namespace JsTimers
{
    public abstract class Timer
    {
        readonly long             _delay;
        readonly Action           _callback;
        readonly int              _id;

        internal long nextExecutionTime = 0;

        protected volatile bool _destroyed;
        volatile           bool _isRef;

        public bool Destroyed
        {
            get => _destroyed;
            protected set
            {
                if (_destroyed == value)
                {
                    return;
                }

                _destroyed = value;

                if (_isRef)
                {
                    if (_destroyed)
                    {
                        TimerManager.UnRefMe(this);
                    }
                    else
                    {
                        TimerManager.RefMe(this);
                    }
                }
            }
        }

        public int Id => _id;

        public event Action<Exception> OnError;

        protected Timer(Action callback, int delay)
        {
            _delay = MillisecondsToTicks(delay);
            _callback = callback;
            _id = TimerManager.GetId();

            Ref();
            RefreshExecutionTime();
        }

        static long MillisecondsToTicks(int milliseconds)
        {
            return milliseconds * TimeSpan.TicksPerMillisecond;
        }

        public void Ref()
        {
            if (_isRef)
            {
                return;
            }

            _isRef = true;

            if (!_destroyed)
            {
                TimerManager.RefMe(this);
            }
        }

        public void UnRef()
        {
            if (!_isRef)
            {
                return;
            }

            _isRef = false;

            if (!_destroyed)
            {
                TimerManager.UnRefMe(this);
            }
        }

        public bool HasRef()
        {
            return _isRef;
        }

        internal virtual void SafeExecute()
        {
            try
            {
                _callback.Invoke();
            }
            catch (Exception e)
            {
                if (OnError != null)
                {
                    OnError.Invoke(e);
                }
                else if(!TimerManager.RaiseException(this, e))
                {
                    Console.Error.WriteLine(BuildExceptionMessage(e));
                }
            }
        }

        internal void DestroyNow()
        {
            if (!_destroyed)
            {
                Destroyed = true;
            }
        }

        protected void RefreshExecutionTime()
        {
            nextExecutionTime = TimerManager.TicksNow + _delay;
        }

        string BuildExceptionMessage(Exception e)
        {
            return $"Timer#{_id} callback produced an unhandled exception:\n{e}";
        }
    }
}
