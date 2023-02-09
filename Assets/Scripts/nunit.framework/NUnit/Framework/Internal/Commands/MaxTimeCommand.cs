using System.Diagnostics;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
	public class MaxTimeCommand : DelegatingTestCommand
	{
		private int maxTime;

		public MaxTimeCommand(TestCommand innerCommand, int maxTime)
			: base(innerCommand)
		{
			this.maxTime = maxTime;
		}

		public override TestResult Execute(ITestExecutionContext context)
		{
			long timestamp = Stopwatch.GetTimestamp();
			TestResult testResult = innerCommand.Execute(context);
			long num = Stopwatch.GetTimestamp() - timestamp;
			double duration = (double)num / (double)Stopwatch.Frequency;
			testResult.Duration = duration;
			if (testResult.ResultState == ResultState.Success)
			{
				double num2 = testResult.Duration * 1000.0;
				if (num2 > (double)maxTime)
				{
					testResult.SetResult(ResultState.Failure, string.Format("Elapsed time of {0}ms exceeds maximum of {1}ms", num2, maxTime));
				}
			}
			return testResult;
		}
	}
}
