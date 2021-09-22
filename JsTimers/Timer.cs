using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace JsTimers
{
    public abstract class Timer
    {
        readonly          Action _callback;
        internal readonly int    Duration;
        protected         bool   destroyed;

        public bool Destroyed => destroyed;
        public int DelayMilliseconds => Duration;
        public float DelaySeconds => Duration / 1000f;
        public Action Callback => _callback;

        protected Timer(Action callback, int duration)
        {
            _callback = callback;
            Duration = duration;
        }

        internal void Destroy(bool execute)
        {
#if DEBUG
            if (Destroyed)
            {
                throw new Exception("Destroying timer which has already been destroyed");
            }
#endif

            if (execute)
            {
                Execute();
            }

            destroyed = true;
        }

        internal virtual void Execute()
        {
#if DEBUG
            if (Destroyed)
            {
                throw new Exception("Executing timer which has already been destroyed");
            }
#endif

            _callback.Invoke();
        }
    }
}
