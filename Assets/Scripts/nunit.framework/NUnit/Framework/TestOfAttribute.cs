using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class TestOfAttribute : PropertyAttribute
	{
		public TestOfAttribute(Type type)
			: base("TestOf", type.FullName)
		{
		}

		public TestOfAttribute(string typeName)
			: base("TestOf", typeName)
		{
		}
	}
}
