namespace NUnit.Framework.Internal.Execution
{
	public interface IWorkItemDispatcher
	{
		void Dispatch(WorkItem work);

		void CancelRun(bool force);
	}
}
