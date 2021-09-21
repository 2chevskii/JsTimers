
namespace JsTimers.Internal
{
	internal sealed class Timeout : BaseTimer
	{
		public Timeout() => _timer.Elapsed += (obj, args) => ReturnToPool();
	}
}
