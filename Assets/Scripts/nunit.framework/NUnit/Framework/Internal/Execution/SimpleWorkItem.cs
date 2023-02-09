using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework.Internal.Execution
{
	public class SimpleWorkItem : WorkItem
	{
		private TestCommand _command;

		public SimpleWorkItem(TestMethod test, ITestFilter filter)
			: base(test)
		{
			_command = ((test.RunState == RunState.Runnable || (test.RunState == RunState.Explicit && filter.IsExplicitMatch(test))) ? CommandBuilder.MakeTestCommand(test) : CommandBuilder.MakeSkipCommand(test));
		}

		protected override void PerformWork()
		{
			try
			{
				base.Result = _command.Execute(base.Context);
			}
			finally
			{
				WorkItemComplete();
			}
		}
	}
}
