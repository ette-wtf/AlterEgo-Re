using System.Diagnostics;

namespace System.Threading
{
	internal struct SpinWait
	{
		private const int step = 10;

		private const int maxTime = 200;

		private static readonly bool isSingleCpu = Environment.ProcessorCount == 1;

		private int ntime;

		public bool NextSpinWillYield
		{
			get
			{
				return isSingleCpu || ntime % 10 == 0;
			}
		}

		public int Count
		{
			get
			{
				return ntime;
			}
		}

		public void SpinOnce()
		{
			ntime++;
			if (isSingleCpu)
			{
				Thread.Sleep((ntime % 10 == 0) ? 1 : 0);
			}
			else if (ntime % 10 == 0)
			{
				Thread.Sleep(1);
			}
			else
			{
				Thread.SpinWait(Math.Min(ntime, 200) << 1);
			}
		}

		public static void SpinUntil(Func<bool> condition)
		{
			SpinWait spinWait = default(SpinWait);
			while (!condition())
			{
				spinWait.SpinOnce();
			}
		}

		public static bool SpinUntil(Func<bool> condition, TimeSpan timeout)
		{
			return SpinUntil(condition, (int)timeout.TotalMilliseconds);
		}

		public static bool SpinUntil(Func<bool> condition, int millisecondsTimeout)
		{
			SpinWait spinWait = default(SpinWait);
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (!condition())
			{
				if (stopwatch.ElapsedMilliseconds > millisecondsTimeout)
				{
					return false;
				}
				spinWait.SpinOnce();
			}
			return true;
		}

		public void Reset()
		{
			ntime = 0;
		}
	}
}
