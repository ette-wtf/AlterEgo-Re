namespace NUnit.Framework.Interfaces
{
	public interface ITestFilter : IXmlNodeBuilder
	{
		bool Pass(ITest test);

		bool IsExplicitMatch(ITest test);
	}
}
