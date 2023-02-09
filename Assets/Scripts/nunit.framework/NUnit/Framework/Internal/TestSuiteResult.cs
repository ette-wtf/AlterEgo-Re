using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class TestSuiteResult : TestResult
	{
		private int _passCount = 0;

		private int _failCount = 0;

		private int _skipCount = 0;

		private int _inconclusiveCount = 0;

		private List<ITestResult> _children;

		public override int FailCount
		{
			get
			{
				try
				{
					return _failCount;
				}
				finally
				{
				}
			}
		}

		public override int PassCount
		{
			get
			{
				try
				{
					return _passCount;
				}
				finally
				{
				}
			}
		}

		public override int SkipCount
		{
			get
			{
				try
				{
					return _skipCount;
				}
				finally
				{
				}
			}
		}

		public override int InconclusiveCount
		{
			get
			{
				try
				{
					return _inconclusiveCount;
				}
				finally
				{
				}
			}
		}

		public override bool HasChildren
		{
			get
			{
				return _children.Count != 0;
			}
		}

		public override IEnumerable<ITestResult> Children
		{
			get
			{
				return _children;
			}
		}

		public TestSuiteResult(TestSuite suite)
			: base(suite)
		{
			_children = new List<ITestResult>();
		}

		public virtual void AddResult(ITestResult result)
		{
			IList<ITestResult> list = Children as IList<ITestResult>;
			if (list != null)
			{
				list.Add(result);
				try
				{
					if (base.ResultState != ResultState.Cancelled)
					{
						switch (result.ResultState.Status)
						{
						case TestStatus.Passed:
							if (base.ResultState.Status == TestStatus.Inconclusive)
							{
								SetResult(ResultState.Success);
							}
							break;
						case TestStatus.Failed:
							if (base.ResultState.Status != TestStatus.Failed)
							{
								SetResult(ResultState.ChildFailure, TestResult.CHILD_ERRORS_MESSAGE);
							}
							break;
						case TestStatus.Skipped:
							if (result.ResultState.Label == "Ignored" && (base.ResultState.Status == TestStatus.Inconclusive || base.ResultState.Status == TestStatus.Passed))
							{
								SetResult(ResultState.Ignored, TestResult.CHILD_IGNORE_MESSAGE);
							}
							break;
						}
					}
					InternalAssertCount += result.AssertCount;
					_passCount += result.PassCount;
					_failCount += result.FailCount;
					_skipCount += result.SkipCount;
					_inconclusiveCount += result.InconclusiveCount;
					return;
				}
				finally
				{
				}
			}
			throw new NotSupportedException("cannot add results to Children");
		}
	}
}
