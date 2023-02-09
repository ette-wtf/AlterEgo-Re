using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
	public class SkipCommand : TestCommand
	{
		public SkipCommand(Test test)
			: base(test)
		{
		}

		public override TestResult Execute(ITestExecutionContext context)
		{
			TestResult currentResult = context.CurrentResult;
			switch (base.Test.RunState)
			{
			default:
				currentResult.SetResult(ResultState.Skipped, GetSkipReason());
				break;
			case RunState.Ignored:
				currentResult.SetResult(ResultState.Ignored, GetSkipReason());
				break;
			case RunState.Explicit:
				currentResult.SetResult(ResultState.Explicit, GetSkipReason());
				break;
			case RunState.NotRunnable:
				currentResult.SetResult(ResultState.NotRunnable, GetSkipReason(), GetProviderStackTrace());
				break;
			}
			return currentResult;
		}

		private string GetSkipReason()
		{
			return (string)base.Test.Properties.Get("_SKIPREASON");
		}

		private string GetProviderStackTrace()
		{
			return (string)base.Test.Properties.Get("_PROVIDERSTACKTRACE");
		}
	}
}
