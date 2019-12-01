using JsTimers;
using System;
using System.IO;
using System.Text;

namespace JsTimers.Tests
{
	public class Program
	{
		private static StringBuilder builder = new StringBuilder();

		public static void Main(string[] args)
		{
			Log("Starting tests at " + DateTime.Now);

			var msDelay = 2750u;
			var sDelay = 2.75f;
			DateTime dt = default(DateTime);
			DateTime _dt = default(DateTime);

			Log("Desired delay - 2.75sec (2750ms)");

			setTimeout(() =>
			{
				var __dt = DateTime.Now;
				var delay = (__dt - dt).TotalMilliseconds;
				var var = Math.Abs(delay - msDelay);
				Log("Finished milliseconds timeout test with " + delay + "ms delay, variation is " + var + $"ms, ({var / msDelay}%)");
			}, msDelay);
			dt = DateTime.Now;

			setTimeout(() =>
			{
				var __dt = DateTime.Now;
				var delay = (__dt - _dt).TotalSeconds;
				var var = Math.Abs(delay - sDelay);
				Log("Finished seconds timeout test with " + delay + "s delay, variation is " + var + $"s, ({var / sDelay}%)");
			}, sDelay);
			_dt = DateTime.Now;

			var cycles = 0;
			BaseTimer _int = null;
			_int =
			setInterval(() =>
			{
				Log("Cycle " + ++cycles);

				if(cycles == 3)
				{
					clear(_int, true);
				}

				if(cycles == 4)
				{
					Log("Interval test finished!", true);
				}
			}, msDelay, true);

			Console.ReadKey();
		}

		private static void Log(string text, bool end = false)
		{
			builder.AppendLine(text);

			Console.WriteLine(text);

			if(end)
			{
				File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "tests.log"), builder.ToString());
				Environment.Exit(0);
			}
		}

		private static BaseTimer setTimeout(Action callback, uint to) => BaseTimer.SetTimeout(callback, to);
		private static BaseTimer setTimeout(Action callback, float to) => BaseTimer.SetTimeout(callback, to);

		private static bool clear(BaseTimer timer, bool execCallback = false) => BaseTimer.ClearTimer(timer, execCallback);

		private static BaseTimer setInterval(Action callback, uint to, bool execImmediately = false) => BaseTimer.SetInterval(callback, to, execImmediately);
		//private static BaseTimer setInterval(Action callback, float to, bool execImmediately = false) => BaseTimer.SetInterval(callback, to, execImmediately);
	}
}
