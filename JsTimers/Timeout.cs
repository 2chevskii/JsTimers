using System;

namespace JsTimers
{
    public class Timeout : Timer
    {
        readonly bool _shouldRepeat;

        internal Timeout(Action callback, int duration, bool shouldRepeat) : base(callback, duration)
        {
            _shouldRepeat = shouldRepeat;
        }

        internal override void Execute()
        {
            destroyed = !_shouldRepeat;
            base.Execute();
        }

        public void Refresh()
        {
            if (!destroyed)
            {
                return;
            }

            destroyed = false;
            TimerManager.RunTimeout(this);
        }
    }
}
