using System;
using System.Collections.Generic;

namespace NUnit.Framework.Interfaces
{
	public interface ITestResult : IXmlNodeBuilder
	{
		ResultState ResultState { get; }

		string Name { get; }

		string FullName { get; }

		double Duration { get; }

		DateTime StartTime { get; }

		DateTime EndTime { get; }

		string Message { get; }

		string StackTrace { get; }

		int AssertCount { get; }

		int FailCount { get; }

		int PassCount { get; }

		int SkipCount { get; }

		int InconclusiveCount { get; }

		bool HasChildren { get; }

		IEnumerable<ITestResult> Children { get; }

		ITest Test { get; }

		string Output { get; }
	}
}
