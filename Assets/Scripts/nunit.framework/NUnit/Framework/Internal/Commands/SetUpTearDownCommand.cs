using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework.Internal.Commands
{
	public class SetUpTearDownCommand : DelegatingTestCommand
	{
		private IList<SetUpTearDownItem> _setUpTearDownItems;

		public SetUpTearDownCommand(TestCommand innerCommand)
			: base(innerCommand)
		{
			Guard.ArgumentValid(innerCommand.Test is TestMethod, "SetUpTearDownCommand may only apply to a TestMethod", "innerCommand");
			Guard.OperationValid(base.Test.TypeInfo != null, "TestMethod must have a non-null TypeInfo");
			_setUpTearDownItems = CommandBuilder.BuildSetUpTearDownList(base.Test.TypeInfo.Type, typeof(SetUpAttribute), typeof(TearDownAttribute));
		}

		public override TestResult Execute(ITestExecutionContext context)
		{
			try
			{
				int num = _setUpTearDownItems.Count;
				while (num > 0)
				{
					_setUpTearDownItems[--num].RunSetUp(context);
				}
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
			finally
			{
				if (context.ExecutionStatus != TestExecutionStatus.AbortRequested)
				{
					for (int num = 0; num < _setUpTearDownItems.Count; num++)
					{
						_setUpTearDownItems[num].RunTearDown(context);
					}
				}
			}
			return context.CurrentResult;
		}
	}
}
