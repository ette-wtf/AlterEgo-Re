using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces
{
	public interface ITestBuilder
	{
		IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite);
	}
}
