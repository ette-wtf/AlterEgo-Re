namespace NUnit.Framework.Interfaces
{
	public interface ITestData
	{
		string TestName { get; }

		RunState RunState { get; }

		object[] Arguments { get; }

		IPropertyBag Properties { get; }
	}
}
