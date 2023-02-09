using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces
{
	public interface ISuiteBuilder
	{
		bool CanBuildFrom(ITypeInfo typeInfo);

		TestSuite BuildFrom(ITypeInfo typeInfo);
	}
}
