namespace NUnit.Framework.Interfaces
{
	public interface ITestListener
	{
		void TestStarted(ITest test);

		void TestFinished(ITestResult result);

		void TestOutput(TestOutput output);
	}
}
