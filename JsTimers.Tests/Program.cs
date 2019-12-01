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
			Log("Starting test");
			var random = new Random();

			var rndMS = (uint)random.Next(0, 5000);
			var rndS = (float)(random.NextDouble() * 5);

			var rndCycles = random.Next(1, 3);

			Log("Random ms: " + rndMS);
			Log("Random sec: " + rndS);
			Log("Random cycles: " + rndCycles);

			var startTime = DateTime.Now;

			double calculateDelay(DateTime now)
			{
				return (now - startTime).TotalMilliseconds;
			}

			Log("Start time: " + startTime);

			var cycle1 = 1;
			var cycle2 = 1;

			setTimeout(() =>
			{
				Log("Timeout 1 delay: " + calculateDelay(DateTime.Now));
			}, rndMS);

			setTimeout(() =>
			{
				Log("Timeout 2 delay: " + calculateDelay(DateTime.Now));
			}, rndS);

			setInterval(() =>
			{
				Log($"Interval 1 cycle {cycle1++}: " + calculateDelay(DateTime.Now));
			}, rndMS);

			setInterval(() =>
			{
				if(cycle2 >=rndCycles && cycle1>=rndCycles)
				{
					Log("Test finished: " + DateTime.Now, true);
					Environment.Exit(0);
				}
				Log($"Interval 1 cycle {cycle2++}: " + calculateDelay(DateTime.Now));
			},rndS);

			Console.ReadKey();
		}

		private static void Log(string text,bool end=false)
		{
			builder.AppendLine(text);

			Console.WriteLine(text);

			if(end)
			{
				File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "tests.log"), builder.ToString());
			}
		}

		private static BaseTimer setTimeout(Action callback, uint to) => BaseTimer.SetTimeout(callback, to);
		private static BaseTimer setTimeout(Action callback, float to) => BaseTimer.SetTimeout(callback, to);

		private static BaseTimer setInterval(Action callback, uint to, bool execImmediately = false) => BaseTimer.SetInterval(callback, to, execImmediately);
		private static BaseTimer setInterval(Action callback, float to, bool execImmediately = false) => BaseTimer.SetInterval(callback, to, execImmediately);
	}
}
