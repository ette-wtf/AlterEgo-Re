using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
	public class ApplyChangesToContextCommand : DelegatingTestCommand
	{
		private IEnumerable<IApplyToContext> _changes;

		public ApplyChangesToContextCommand(TestCommand innerCommand, IEnumerable<IApplyToContext> changes)
			: base(innerCommand)
		{
			_changes = changes;
		}

		public void ApplyChanges(ITestExecutionContext context)
		{
			foreach (IApplyToContext change in _changes)
			{
				change.ApplyToContext(context);
			}
		}

		public override TestResult Execute(ITestExecutionContext context)
		{
			try
			{
				ApplyChanges(context);
				context.CurrentResult = innerCommand.Execute(context);
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException)
				{
					Thread.ResetAbort();
				}
				context.CurrentResult.RecordException(ex);
			}
			return context.CurrentResult;
		}
	}
}
