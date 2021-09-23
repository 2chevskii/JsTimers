using System;

namespace JsTimers
{
    /// <summary>
    /// Timer which is guaranteed to execute at most once.
    /// Has higher execution preference than <see cref="Timeout"/>
    /// </summary>
    public sealed class Immediate : Timer
    {
        internal Immediate(Action callback) : base(callback, 0) { }

        internal override void SafeExecute()
        {
            base.SafeExecute();
            Destroyed = true;
        }
    }
}
