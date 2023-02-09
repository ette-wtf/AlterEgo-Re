using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class SetUpFixture : TestSuite, IDisposableFixture
	{
		public SetUpFixture(ITypeInfo type)
			: base(type)
		{
			base.Name = type.Namespace;
			if (base.Name == null)
			{
				base.Name = "[default namespace]";
			}
			int num = base.Name.LastIndexOf('.');
			if (num > 0)
			{
				base.Name = base.Name.Substring(num + 1);
			}
			CheckSetUpTearDownMethods(typeof(OneTimeSetUpAttribute));
			CheckSetUpTearDownMethods(typeof(OneTimeTearDownAttribute));
		}
	}
}
