using NUnit.Framework.Interfaces;

namespace NUnit.Framework
{
	public interface ITestAction
	{
		ActionTargets Targets { get; }

		void BeforeTest(ITest test);

		void AfterTest(ITest test);
	}
}
