#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace NUnit.Framework.Internal.Commands
{
	public class TestActionCommand : DelegatingTestCommand
	{
		private IList<TestActionItem> _actions = new List<TestActionItem>();

		public TestActionCommand(TestCommand innerCommand)
			: base(innerCommand)
		{
			Guard.ArgumentValid(innerCommand.Test is TestMethod, "TestActionCommand may only apply to a TestMethod", "innerCommand");
		}

		public override TestResult Execute(ITestExecutionContext context)
		{
			if (base.Test.Fixture == null)
			{
				base.Test.Fixture = context.TestObject;
			}
			foreach (ITestAction upstreamAction in context.UpstreamActions)
			{
				Debug.Assert(upstreamAction.Targets == ActionTargets.Default || (upstreamAction.Targets & ActionTargets.Test) == ActionTargets.Test, "Invalid target on upstream action: " + upstreamAction.Targets);
				_actions.Add(new TestActionItem(upstreamAction));
			}
			ITestAction[] actionsFromAttributeProvider = ActionsHelper.GetActionsFromAttributeProvider(((TestMethod)base.Test).Method.MethodInfo);
			foreach (ITestAction current in actionsFromAttributeProvider)
			{
				if (current.Targets == ActionTargets.Default || (current.Targets & ActionTargets.Test) == ActionTargets.Test)
				{
					_actions.Add(new TestActionItem(current));
				}
			}
			try
			{
				for (int j = 0; j < _actions.Count; j++)
				{
					_actions[j].BeforeTest(base.Test);
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
					for (int j = _actions.Count - 1; j >= 0; j--)
					{
						_actions[j].AfterTest(base.Test);
					}
				}
			}
			return context.CurrentResult;
		}
	}
}
