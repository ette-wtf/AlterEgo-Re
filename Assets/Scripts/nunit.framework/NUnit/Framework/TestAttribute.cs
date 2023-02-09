using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class TestAttribute : NUnitAttribute, ISimpleTestBuilder, IApplyToTest, IImplyFixture
	{
		private object _expectedResult;

		private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

		public string Description { get; set; }

		public string Author { get; set; }

		public Type TestOf { get; set; }

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

		public bool HasExpectedResult { get; private set; }

		public void ApplyToTest(Test test)
		{
			if (!test.Properties.ContainsKey("Description") && Description != null)
			{
				test.Properties.Set("Description", Description);
			}
			if (!test.Properties.ContainsKey("Author") && Author != null)
			{
				test.Properties.Set("Author", Author);
			}
			if (!test.Properties.ContainsKey("TestOf") && (object)TestOf != null)
			{
				test.Properties.Set("TestOf", TestOf.FullName);
			}
		}

		public TestMethod BuildFrom(IMethodInfo method, Test suite)
		{
			TestCaseParameters testCaseParameters = null;
			if (HasExpectedResult)
			{
				testCaseParameters = new TestCaseParameters();
				testCaseParameters.ExpectedResult = ExpectedResult;
			}
			return _builder.BuildTestMethod(method, suite, testCaseParameters);
		}
	}
}
