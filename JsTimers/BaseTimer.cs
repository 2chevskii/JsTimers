#pragma warning disable CA1051

using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using JsTimers.Internal;

namespace JsTimers
{
	public abstract class BaseTimer : IDisposable
	{
		#region Static fields

		private static readonly HashSet<BaseTimer> pool = new HashSet<BaseTimer>();
		private static readonly Random generator = new Random();

		#endregion

		#region Instance fields

		protected readonly uint _id;
		protected readonly Timer _timer;

		private Action _callback;
		private bool _disposed = false;

		#endregion

		#region Public properties

		public string CallbackName => _callback?.Method?.Name ?? string.Empty;
		public bool InUse { get; protected set; }
		public uint ID => _id;

		#endregion

		#region Constructors / destructors

		protected BaseTimer()
		{
			_timer = new Timer();
			_timer.Elapsed += (obj, args) => ExecuteImmediate();

			do
			{
				_id = (uint)generator.Next((int)uint.MinValue, int.MaxValue);
			} while(pool.Any(t => t._id == _id));
		}

		~BaseTimer() => Dispose(false);

		#endregion

		#region IDisposable implementation

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(!_disposed)
			{
				if(disposing)
				{
					_timer?.Dispose();
				}

				_disposed = true;
			}
		}

		#endregion

		#region Public library functions

		public static BaseTimer SetTimeout(Action callback, uint timeoutMilliseconds) => TryGetFromPool<Timeout>().InitNew(callback, timeoutMilliseconds);
		public static BaseTimer SetTimeout(Action callback, float timeoutSeconds) => SetTimeout(callback, Convert.ToUInt32(timeoutSeconds * 1000));

		public static BaseTimer SetInterval(Action callback, uint timeoutMilliseconds, bool executeImmediately = false) 
			=> TryGetFromPool<Interval>().InitNew(callback, timeoutMilliseconds, executeImmediately);
		public static BaseTimer SetInterval(Action callback, float timeoutSeconds, bool executeImmediately = false) 
			=> SetInterval(callback, Convert.ToUInt32(timeoutSeconds * 1000), executeImmediately);

		public static bool ClearTimer(BaseTimer timer, bool executeCallback = false)
		{
			if(timer is null)
			{
				return false;
			}

			if(!timer.InUse)
			{
				return false;
			}

			timer.ReturnToPool();

			if(executeCallback)
			{
				timer.ExecuteImmediate();
			}

			return true;
		}

		#endregion

		#region Helper methods

		private static T TryGetFromPool<T>() where T : BaseTimer, new()
		{
			var timer = (T)pool.FirstOrDefault(t => t is T && !t.InUse);

			if(timer is null)
			{
				timer = new T();
				pool.Add(timer);
			}

			return timer;
		}

		#endregion

		#region Instance handling

		protected virtual BaseTimer InitNew(Action callback, uint timeMS)
		{
			InUse = true;
			_callback = callback;
			InitTimer(timeMS);
			return this;
		}

		protected void ReturnToPool()
		{
			_timer.Stop();
			InUse = false;
		}

		protected void ExecuteImmediate()
		{
			if(_callback is null)
			{
				throw new NullReferenceException("Trying to invoke null callback in timer with id: " + _id);
			}
			else
			{
				try
				{
					_callback.Invoke();
				}
				catch(Exception ex)
				{
					throw new InvalidOperationException("Callback execution produced an exception in timer with id: " + _id, ex);
				}
			}
		}

		private void InitTimer(uint intervalMS)
		{
			_timer.Interval = intervalMS;
			if(!_timer.Enabled)
			{
				_timer.Start();
			}
		}

		#endregion
	}
}

#pragma warning restore CA1051
