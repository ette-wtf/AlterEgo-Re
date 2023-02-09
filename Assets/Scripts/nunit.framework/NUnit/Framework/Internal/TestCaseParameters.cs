using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class TestCaseParameters : TestParameters, ITestCaseData, ITestData, IApplyToTest
	{
		private object _expectedResult;

		public object ExpectedResult
		{
			get
			{
				return _expectedResult;
			}
			set
			{
				_expectedResult = value;
				HasExpectedResult = true;
			}
		}

		public bool HasExpectedResult { get; set; }

		public TestCaseParameters()
		{
		}

		public TestCaseParameters(Exception exception)
			: base(exception)
		{
		}

		public TestCaseParameters(object[] args)
			: base(args)
		{
		}

		public TestCaseParameters(ITestCaseData data)
			: base(data)
		{
			if (data.HasExpectedResult)
			{
				ExpectedResult = data.ExpectedResult;
			}
		}
	}
}
