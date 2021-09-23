using System;

namespace JsTimers
{
    public class Timeout : Timer
    {
        readonly bool _repeating;

        internal Timeout(Action callback, int delay, bool repeating) : base(callback, delay)
        {
            _repeating = repeating;
        }

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
        }
    }
}
