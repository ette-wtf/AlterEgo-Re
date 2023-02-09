using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
	public class TestMethodCommand : TestCommand
	{
		private readonly TestMethod testMethod;

		private readonly object[] arguments;

		public TestMethodCommand(TestMethod testMethod)
			: base(testMethod)
		{
			this.testMethod = testMethod;
			arguments = testMethod.Arguments;
		}

		public override TestResult Execute(ITestExecutionContext context)
		{
			object actual = RunTestMethod(context);
			if (testMethod.HasExpectedResult)
			{
				Assert.AreEqual(testMethod.ExpectedResult, actual);
			}
			context.CurrentResult.SetResult(ResultState.Success);
			return context.CurrentResult;
		}

		private object RunTestMethod(ITestExecutionContext context)
		{
			return RunNonAsyncTestMethod(context);
		}

		private object RunNonAsyncTestMethod(ITestExecutionContext context)
		{
			return testMethod.Method.Invoke(context.TestObject, arguments);
		}
	}
}
