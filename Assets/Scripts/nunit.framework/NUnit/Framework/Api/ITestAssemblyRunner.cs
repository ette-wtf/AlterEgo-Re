using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Api
{
	public interface ITestAssemblyRunner
	{
		ITest LoadedTest { get; }

		ITestResult Result { get; }

		bool IsTestLoaded { get; }

		bool IsTestRunning { get; }

		bool IsTestComplete { get; }

		ITest Load(string assemblyName, IDictionary<string, object> settings);

		ITest Load(Assembly assembly, IDictionary<string, object> settings);

		int CountTestCases(ITestFilter filter);

		ITestResult Run(ITestListener listener, ITestFilter filter);

		void RunAsync(ITestListener listener, ITestFilter filter);

		bool WaitForCompletion(int timeout);

		void StopRun(bool force);
	}
}
