using System;

namespace JsTimers
{
    public class Immediate : Timer
    {
        internal Immediate(Action callback) : base(callback, 0)
        {

        }

        internal override void Execute()
        {
            destroyed = true;
            base.Execute();
        }

        public void Refresh()
        {
            if (!destroyed)
            {
                return;
            }

            destroyed = false;
            TimerManager.RunImmediate(this);
        }
    }
}
