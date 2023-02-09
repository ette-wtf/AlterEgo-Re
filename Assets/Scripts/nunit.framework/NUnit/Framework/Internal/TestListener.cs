using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class TestListener : ITestListener
	{
		public static ITestListener NULL
		{
			get
			{
				return new TestListener();
			}
		}

		public void TestStarted(ITest test)
		{
		}

		public void TestFinished(ITestResult result)
		{
		}

		public void TestOutput(TestOutput output)
		{
		}

		private TestListener()
		{
		}
	}
}
