using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JsTimers
{
    static class Utility
    {
        public static int SecondsToMilliseconds(float seconds)
        {
            return (int)(seconds * 1000f);
        }
    }
}
