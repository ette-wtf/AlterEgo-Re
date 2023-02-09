using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class TestFixture : TestSuite, IDisposableFixture
	{
		public TestFixture(ITypeInfo fixtureType)
			: base(fixtureType)
		{
			CheckSetUpTearDownMethods(typeof(OneTimeSetUpAttribute));
			CheckSetUpTearDownMethods(typeof(OneTimeTearDownAttribute));
			CheckSetUpTearDownMethods(typeof(SetUpAttribute));
			CheckSetUpTearDownMethods(typeof(TearDownAttribute));
		}
	}
}
