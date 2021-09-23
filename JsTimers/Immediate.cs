using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace JsTimers
{
    public class Immediate : Timer
    {
        internal Immediate(Action callback) : base(callback, 0) { }

        internal override void SafeExecute()
        {
            base.SafeExecute();
            Destroyed = true;
        }
    }
}
