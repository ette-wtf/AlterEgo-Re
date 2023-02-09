using System.Collections.Generic;

namespace NUnit.Framework.Interfaces
{
	public interface ITest : IXmlNodeBuilder
	{
		string Id { get; }

		string Name { get; }

		string FullName { get; }

		string ClassName { get; }

		string MethodName { get; }

		ITypeInfo TypeInfo { get; }

		IMethodInfo Method { get; }

		RunState RunState { get; }

		int TestCaseCount { get; }

		IPropertyBag Properties { get; }

		ITest Parent { get; }

		bool IsSuite { get; }

		bool HasChildren { get; }

		IList<ITest> Tests { get; }

		object Fixture { get; }
	}
}
