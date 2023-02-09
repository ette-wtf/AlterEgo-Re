using System.Threading;

namespace NUnit.Framework.Internal.Execution
{
	public class SimpleWorkItemDispatcher : IWorkItemDispatcher
	{
		private WorkItem _topLevelWorkItem;

		private Thread _runnerThread;

		private object cancelLock = new object();

		public void Dispatch(WorkItem work)
		{
			if (_topLevelWorkItem != null)
			{
				work.Execute();
				return;
			}
			_topLevelWorkItem = work;
			_runnerThread = new Thread(RunnerThreadProc);
			if (work.TargetApartment == ApartmentState.STA)
			{
				_runnerThread.SetApartmentState(ApartmentState.STA);
			}
			_runnerThread.Start();
		}

		private void RunnerThreadProc()
		{
			_topLevelWorkItem.Execute();
		}

		public void CancelRun(bool force)
		{
			lock (cancelLock)
			{
				if (_topLevelWorkItem != null)
				{
					_topLevelWorkItem.Cancel(force);
					if (force)
					{
						_topLevelWorkItem = null;
					}
				}
			}
		}
	}
}
