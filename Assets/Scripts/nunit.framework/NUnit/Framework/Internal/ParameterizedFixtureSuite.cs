using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class ParameterizedFixtureSuite : TestSuite
	{
		private bool _genericFixture;

		public override string TestType
		{
			get
			{
				return _genericFixture ? "GenericFixture" : "ParameterizedFixture";
			}
		}

		public ParameterizedFixtureSuite(ITypeInfo typeInfo)
			: base(typeInfo.Namespace, typeInfo.GetDisplayName())
		{
			_genericFixture = typeInfo.ContainsGenericParameters;
		}
	}
}
