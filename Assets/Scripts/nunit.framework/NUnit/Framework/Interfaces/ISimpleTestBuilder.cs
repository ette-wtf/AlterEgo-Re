using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces
{
	public interface ISimpleTestBuilder
	{
		TestMethod BuildFrom(IMethodInfo method, Test suite);
	}
}
