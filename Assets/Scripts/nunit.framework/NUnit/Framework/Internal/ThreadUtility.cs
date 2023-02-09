using System.Threading;

namespace NUnit.Framework.Internal
{
	public static class ThreadUtility
	{
		public static void Kill(Thread thread)
		{
			Kill(thread, null);
		}

		public static void Kill(Thread thread, object stateInfo)
		{
			try
			{
				if (stateInfo == null)
				{
					thread.Abort();
				}
				else
				{
					thread.Abort(stateInfo);
				}
			}
			catch (ThreadStateException)
			{
				thread.Resume();
			}
			if ((thread.ThreadState & ThreadState.WaitSleepJoin) != 0)
			{
				thread.Interrupt();
			}
		}
	}
}
