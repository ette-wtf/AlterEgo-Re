using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces
{
	public interface IFixtureBuilder
	{
		IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo);
	}
}
