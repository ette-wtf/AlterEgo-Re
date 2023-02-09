using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces
{
	public interface ITestCaseBuilder
	{
		bool CanBuildFrom(IMethodInfo method, Test suite);

		Test BuildFrom(IMethodInfo method, Test suite);
	}
}
