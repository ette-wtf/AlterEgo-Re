using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class TestCaseResult : TestResult
	{
		public override int FailCount
		{
			get
			{
				return (base.ResultState.Status == TestStatus.Failed) ? 1 : 0;
			}
		}

		public override int PassCount
		{
			get
			{
				return (base.ResultState.Status == TestStatus.Passed) ? 1 : 0;
			}
		}

		public override int SkipCount
		{
			get
			{
				return (base.ResultState.Status == TestStatus.Skipped) ? 1 : 0;
			}
		}

		public override int InconclusiveCount
		{
			get
			{
				return (base.ResultState.Status == TestStatus.Inconclusive) ? 1 : 0;
			}
		}

		public override bool HasChildren
		{
			get
			{
				return false;
			}
		}

		public override IEnumerable<ITestResult> Children
		{
			get
			{
				return new ITestResult[0];
			}
		}

		public TestCaseResult(TestMethod test)
			: base(test)
		{
		}
	}
}
