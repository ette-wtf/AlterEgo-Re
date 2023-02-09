using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework.Internal
{
	public interface ITestExecutionContext
	{
		Test CurrentTest { get; set; }

		DateTime StartTime { get; set; }

		long StartTicks { get; set; }

		TestResult CurrentResult { get; set; }

		TextWriter OutWriter { get; }

		object TestObject { get; set; }

		string WorkDirectory { get; set; }

		bool StopOnError { get; set; }

		TestExecutionStatus ExecutionStatus { get; set; }

		IWorkItemDispatcher Dispatcher { get; set; }

		ParallelScope ParallelScope { get; set; }

		string WorkerId { get; }

		Randomizer RandomGenerator { get; }

		int TestCaseTimeout { get; set; }

		List<ITestAction> UpstreamActions { get; }

		CultureInfo CurrentCulture { get; set; }

		CultureInfo CurrentUICulture { get; set; }

		ValueFormatter CurrentValueFormatter { get; }

		bool IsSingleThreaded { get; set; }

		void IncrementAssertCount();

		void AddFormatter(ValueFormatterFactory formatterFactory);
	}
}
