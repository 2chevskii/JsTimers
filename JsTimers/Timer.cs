using System;

namespace JsTimers
{
    public abstract class Timer
    {
        readonly long _delay;
        readonly Action _callback;
        readonly int _id;

        internal long nextExecutionTime;

        protected volatile bool _destroyed;
        volatile bool _isRef;

        /// <summary>
        /// Indicates whether the timer has been destroyed (executed set number of times, or was destroyed manually)
        /// </summary>
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

        /// <summary>
        /// Unique id for timer
        /// </summary>
        public int Id => _id;

        /// <summary>
        /// Fired whenever an exception is thrown inside of the timer callback. Suppresses stderr throw if at least one subscriber is present
        /// </summary>
        public event Action<Exception> OnError;

        protected internal Timer(Action callback, int delay)
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

        #region Public API

        /// <summary>
        /// Sets <see cref="HasRef" /> value to <see langword="true" />
        /// For detailed explanation see <seealso cref="HasRef"/>
        /// </summary>
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

        /// <summary>
        /// Sets <see cref="HasRef" /> value to <see langword="false" />
        /// For detailed explanation see <seealso cref="HasRef"/>
        /// </summary>
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

        /// <summary>
        /// Indicates whether the timer has a reference in <see cref="TimerManager"/>.
        /// If it does - application will be prevent from exiting, until reference is unset or timer is destroyed
        /// </summary>
        /// <returns></returns>
        public bool HasRef()
        {
            return _isRef;
        }

        #endregion

        #region Internal API

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
                else if (!TimerManager.RaiseException(this, e))
                {
                    var defaultColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine(BuildExceptionMessage(e));
                    Console.ForegroundColor = defaultColor;
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

        protected internal void RefreshExecutionTime()
        {
            nextExecutionTime = TimerManager.TicksNow + _delay;
        }

        #endregion

        string BuildExceptionMessage(Exception e)
        {
            return $"Timer#{_id} callback produced an unhandled exception:\n{e}";
        }
    }
}
