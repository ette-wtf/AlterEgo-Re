namespace NUnit.Framework.Interfaces
{
	public interface ITestCaseData : ITestData
	{
		object ExpectedResult { get; }

		bool HasExpectedResult { get; }
	}
}
