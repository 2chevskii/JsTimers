using System;
using System.Diagnostics;

namespace JsTimers
{
    static class DebugLogger
    {
        [Conditional("DEBUG")]
        public static void Log(string message, params object[] args)
        {
            WriteLog(message, args);
        }

        [Conditional("DEBUG")]
        public static void LogWarning(string message, params object[] args)
        {
            WriteLog(message, args, ConsoleColor.Yellow);
        }

        [Conditional("DEBUG")]
        public static void LogError(string message, params object[] args)
        {
            WriteLog(message, args, ConsoleColor.Red);
        }

        static void WriteLog(string format, object[] args, ConsoleColor color = ConsoleColor.Gray)
        {
            var message = string.Format(format, args);
            WriteWithColor(message, color);
        }

        static void WriteWithColor(string text, ConsoleColor color)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = defaultColor;
        }
    }
}
