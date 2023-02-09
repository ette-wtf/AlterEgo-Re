using System.Threading;

namespace NUnit.Framework.Internal.Execution
{
	public class CountdownEvent
	{
		private int _initialCount;

		private int _remainingCount;

		private object _lock = new object();

		private ManualResetEvent _event = new ManualResetEvent(false);

		public int InitialCount
		{
			get
			{
				return _initialCount;
			}
		}

		public int CurrentCount
		{
			get
			{
				return _remainingCount;
			}
		}

		public CountdownEvent(int initialCount)
		{
			_initialCount = (_remainingCount = initialCount);
		}

		public void Signal()
		{
			lock (_lock)
			{
				if (--_remainingCount == 0)
				{
					_event.Set();
				}
			}
		}

		public void Wait()
		{
			_event.WaitOne();
		}
	}
}
