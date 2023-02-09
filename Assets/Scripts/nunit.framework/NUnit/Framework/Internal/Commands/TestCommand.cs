namespace NUnit.Framework.Internal.Commands
{
	public abstract class TestCommand
	{
		public Test Test { get; private set; }

		public TestCommand(Test test)
		{
			Test = test;
		}

		public abstract TestResult Execute(ITestExecutionContext context);
	}
}
