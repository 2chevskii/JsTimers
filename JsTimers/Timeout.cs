using System;

namespace JsTimers
{
    /// <summary>
    /// Timer which can be ran one or multiple times and restarted on demand
    /// </summary>
    public sealed class Timeout : Timer
    {
        readonly bool _repeating;

        /// <summary>
        /// Indicates whether timer will be executed multiple times
        /// </summary>
        public bool IsInterval => _repeating;

        internal Timeout(Action callback, int delay, bool repeating) : base(callback, delay)
        {
            _repeating = repeating;
        }

        /// <summary>
        /// Resets time left to next execution. If timer has been destroyed, it will be restarted
        /// </summary>
        public void Refresh()
        {
            RefreshExecutionTime();

            if (_destroyed)
            {
                TimerManager.Requeue(this);
                Destroyed = false;
            }
        }

        internal override void SafeExecute()
        {
            base.SafeExecute();
            if (!_repeating)
            {
                Destroyed = true;
            }
            else
            {
                RefreshExecutionTime();
            }
        }
    }
}
