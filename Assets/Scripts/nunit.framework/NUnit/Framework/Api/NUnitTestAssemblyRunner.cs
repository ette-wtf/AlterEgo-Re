using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework.Api
{
	public class NUnitTestAssemblyRunner : ITestAssemblyRunner
	{
		private static Logger log = InternalTrace.GetLogger("DefaultTestAssemblyRunner");

		private ITestAssemblyBuilder _builder;

		private ManualResetEvent _runComplete = new ManualResetEvent(false);

		private TextWriter _savedOut;

		private TextWriter _savedErr;

		public ITest LoadedTest { get; protected set; }

		public ITestResult Result
		{
			get
			{
				return (TopLevelWorkItem == null) ? null : TopLevelWorkItem.Result;
			}
		}

		public bool IsTestLoaded
		{
			get
			{
				return LoadedTest != null;
			}
		}

		public bool IsTestRunning
		{
			get
			{
				return TopLevelWorkItem != null && TopLevelWorkItem.State == WorkItemState.Running;
			}
		}

		public bool IsTestComplete
		{
			get
			{
				return TopLevelWorkItem != null && TopLevelWorkItem.State == WorkItemState.Complete;
			}
		}

		protected IDictionary<string, object> Settings { get; set; }

		private WorkItem TopLevelWorkItem { get; set; }

		private TestExecutionContext Context { get; set; }

		public NUnitTestAssemblyRunner(ITestAssemblyBuilder builder)
		{
			_builder = builder;
		}

		public ITest Load(string assemblyName, IDictionary<string, object> settings)
		{
			Settings = settings;
			if (settings.ContainsKey("RandomSeed"))
			{
				Randomizer.InitialSeed = (int)settings["RandomSeed"];
			}
			return LoadedTest = _builder.Build(assemblyName, settings);
		}

		public ITest Load(Assembly assembly, IDictionary<string, object> settings)
		{
			Settings = settings;
			if (settings.ContainsKey("RandomSeed"))
			{
				Randomizer.InitialSeed = (int)settings["RandomSeed"];
			}
			return LoadedTest = _builder.Build(assembly, settings);
		}

		public int CountTestCases(ITestFilter filter)
		{
			if (LoadedTest == null)
			{
				throw new InvalidOperationException("The CountTestCases method was called but no test has been loaded");
			}
			return CountTestCases(LoadedTest, filter);
		}

		public ITestResult Run(ITestListener listener, ITestFilter filter)
		{
			RunAsync(listener, filter);
			WaitForCompletion(-1);
			return Result;
		}

		public void RunAsync(ITestListener listener, ITestFilter filter)
		{
			log.Info("Running tests");
			if (LoadedTest == null)
			{
				throw new InvalidOperationException("The Run method was called but no test has been loaded");
			}
			_runComplete.Reset();
			CreateTestExecutionContext(listener);
			TopLevelWorkItem = WorkItem.CreateWorkItem(LoadedTest, filter);
			TopLevelWorkItem.InitializeContext(Context);
			TopLevelWorkItem.Completed += OnRunCompleted;
			StartRun(listener);
		}

		public bool WaitForCompletion(int timeout)
		{
			return _runComplete.WaitOne(timeout, false);
		}

		public void StopRun(bool force)
		{
			if (IsTestRunning)
			{
				Context.ExecutionStatus = ((!force) ? TestExecutionStatus.StopRequested : TestExecutionStatus.AbortRequested);
				Context.Dispatcher.CancelRun(force);
			}
		}

		private void StartRun(ITestListener listener)
		{
			_savedOut = Console.Out;
			_savedErr = Console.Error;
			Console.SetOut(new EventListenerTextWriter("Out", Console.Out));
			Console.SetError(new EventListenerTextWriter("Error", Console.Error));
			if (!Debugger.IsAttached && Settings.ContainsKey("DebugTests") && (bool)Settings["DebugTests"])
			{
				Debugger.Launch();
			}
			Context.Dispatcher.Dispatch(TopLevelWorkItem);
		}

		private void CreateTestExecutionContext(ITestListener listener)
		{
			Context = new TestExecutionContext();
			if (Settings.ContainsKey("DefaultTimeout"))
			{
				Context.TestCaseTimeout = (int)Settings["DefaultTimeout"];
			}
			if (Settings.ContainsKey("StopOnError"))
			{
				Context.StopOnError = (bool)Settings["StopOnError"];
			}
			if (Settings.ContainsKey("WorkDirectory"))
			{
				Context.WorkDirectory = (string)Settings["WorkDirectory"];
			}
			else
			{
				Context.WorkDirectory = Env.DefaultWorkDirectory;
			}
			Context.Listener = listener;
			Context.Dispatcher = new SimpleWorkItemDispatcher();
		}

		private void OnRunCompleted(object sender, EventArgs e)
		{
			Console.SetOut(_savedOut);
			Console.SetError(_savedErr);
			_runComplete.Set();
		}

		private int CountTestCases(ITest test, ITestFilter filter)
		{
			if (!test.IsSuite)
			{
				return 1;
			}
			int num = 0;
			foreach (ITest test2 in test.Tests)
			{
				if (filter.Pass(test2))
				{
					num += CountTestCases(test2, filter);
				}
			}
			return num;
		}
	}
}
