using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
	public class TheoryResultCommand : DelegatingTestCommand
	{
		public TheoryResultCommand(TestCommand command)
			: base(command)
		{
		}

		public override TestResult Execute(ITestExecutionContext context)
		{
			TestResult testResult = innerCommand.Execute(context);
			if (testResult.ResultState == ResultState.Success)
			{
				if (!testResult.HasChildren)
				{
					testResult.SetResult(ResultState.Failure, "No test cases were provided");
				}
				else
				{
					bool flag = true;
					foreach (TestResult child in testResult.Children)
					{
						if (child.ResultState == ResultState.Success)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						testResult.SetResult(ResultState.Failure, "All test cases were inconclusive");
					}
				}
			}
			return testResult;
		}
	}
}
