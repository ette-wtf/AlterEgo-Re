using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework.Internal.Execution
{
	public class CompositeWorkItem : WorkItem
	{
		private class WorkItemOrderComparer : IComparer<WorkItem>
		{
			public int Compare(WorkItem x, WorkItem y)
			{
				int num = int.MaxValue;
				int value = int.MaxValue;
				if (x.Test.Properties.ContainsKey("Order"))
				{
					num = (int)x.Test.Properties["Order"][0];
				}
				if (y.Test.Properties.ContainsKey("Order"))
				{
					value = (int)y.Test.Properties["Order"][0];
				}
				return num.CompareTo(value);
			}
		}

		private TestSuite _suite;

		private TestSuiteResult _suiteResult;

		private ITestFilter _childFilter;

		private TestCommand _setupCommand;

		private TestCommand _teardownCommand;

		private List<WorkItem> _children;

		private int _countOrder;

		private CountdownEvent _childTestCountdown;

		private object _completionLock = new object();

		private object cancelLock = new object();

		public List<WorkItem> Children
		{
			get
			{
				return _children;
			}
			private set
			{
				_children = value;
			}
		}

		public CompositeWorkItem(TestSuite suite, ITestFilter childFilter)
			: base(suite)
		{
			_suite = suite;
			_suiteResult = base.Result as TestSuiteResult;
			_childFilter = childFilter;
			_countOrder = 0;
		}

		protected override void PerformWork()
		{
			InitializeSetUpAndTearDownCommands();
			if (!CheckForCancellation())
			{
				if (base.Test.RunState == RunState.Explicit && !_childFilter.IsExplicitMatch(base.Test))
				{
					SkipFixture(ResultState.Explicit, GetSkipReason(), null);
				}
				else
				{
					switch (base.Test.RunState)
					{
					default:
						base.Result.SetResult(ResultState.Success);
						CreateChildWorkItems();
						if (_children.Count <= 0)
						{
							break;
						}
						PerformOneTimeSetUp();
						if (!CheckForCancellation())
						{
							switch (base.Result.ResultState.Status)
							{
							case TestStatus.Passed:
								RunChildren();
								return;
							case TestStatus.Inconclusive:
							case TestStatus.Skipped:
							case TestStatus.Failed:
								SkipChildren(_suite, base.Result.ResultState.WithSite(FailureSite.Parent), "OneTimeSetUp: " + base.Result.Message);
								break;
							}
						}
						if (base.Context.ExecutionStatus != TestExecutionStatus.AbortRequested)
						{
							PerformOneTimeTearDown();
						}
						break;
					case RunState.Skipped:
						SkipFixture(ResultState.Skipped, GetSkipReason(), null);
						break;
					case RunState.Ignored:
						SkipFixture(ResultState.Ignored, GetSkipReason(), null);
						break;
					case RunState.NotRunnable:
						SkipFixture(ResultState.NotRunnable, GetSkipReason(), GetProviderStackTrace());
						break;
					}
				}
			}
			WorkItemComplete();
		}

		private bool CheckForCancellation()
		{
			if (base.Context.ExecutionStatus != 0)
			{
				base.Result.SetResult(ResultState.Cancelled, "Test cancelled by user");
				return true;
			}
			return false;
		}

		private void InitializeSetUpAndTearDownCommands()
		{
			List<SetUpTearDownItem> list = ((_suite.TypeInfo != null) ? CommandBuilder.BuildSetUpTearDownList(_suite.TypeInfo.Type, typeof(OneTimeSetUpAttribute), typeof(OneTimeTearDownAttribute)) : new List<SetUpTearDownItem>());
			List<TestActionItem> list2 = new List<TestActionItem>();
			foreach (ITestAction action in base.Actions)
			{
				bool flag = (action.Targets & ActionTargets.Suite) == ActionTargets.Suite || (action.Targets == ActionTargets.Default && !(base.Test is ParameterizedMethodSuite));
				bool flag2 = (action.Targets & ActionTargets.Test) == ActionTargets.Test && !(base.Test is ParameterizedMethodSuite);
				if (flag)
				{
					list2.Add(new TestActionItem(action));
				}
				if (flag2)
				{
					base.Context.UpstreamActions.Add(action);
				}
			}
			_setupCommand = CommandBuilder.MakeOneTimeSetUpCommand(_suite, list, list2);
			_teardownCommand = CommandBuilder.MakeOneTimeTearDownCommand(_suite, list, list2);
		}

		private void PerformOneTimeSetUp()
		{
			try
			{
				_setupCommand.Execute(base.Context);
				base.Context.UpdateContextFromEnvironment();
			}
			catch (Exception innerException)
			{
				if (innerException is NUnitException || innerException is TargetInvocationException)
				{
					innerException = innerException.InnerException;
				}
				base.Result.RecordException(innerException, FailureSite.SetUp);
			}
		}

		private void RunChildren()
		{
			int num = _children.Count;
			if (num == 0)
			{
				throw new InvalidOperationException("RunChildren called but item has no children");
			}
			_childTestCountdown = new CountdownEvent(num);
			foreach (WorkItem child in _children)
			{
				if (CheckForCancellation())
				{
					break;
				}
				child.Completed += OnChildCompleted;
				child.InitializeContext(new TestExecutionContext(base.Context));
				base.Context.Dispatcher.Dispatch(child);
				num--;
			}
			if (num > 0)
			{
				while (num-- > 0)
				{
					CountDownChildTest();
				}
			}
		}

		private void CreateChildWorkItems()
		{
			_children = new List<WorkItem>();
			foreach (ITest test in _suite.Tests)
			{
				if (_childFilter.Pass(test))
				{
					WorkItem workItem = WorkItem.CreateWorkItem(test, _childFilter);
					workItem.WorkerId = base.WorkerId;
					if (workItem.TargetApartment == ApartmentState.Unknown && base.TargetApartment != ApartmentState.Unknown)
					{
						workItem.TargetApartment = base.TargetApartment;
					}
					if (test.Properties.ContainsKey("Order"))
					{
						_children.Insert(0, workItem);
						_countOrder++;
					}
					else
					{
						_children.Add(workItem);
					}
				}
			}
			if (_countOrder != 0)
			{
				SortChildren();
			}
		}

		private void SortChildren()
		{
			_children.Sort(0, _countOrder, new WorkItemOrderComparer());
		}

		private void SkipFixture(ResultState resultState, string message, string stackTrace)
		{
			base.Result.SetResult(resultState.WithSite(FailureSite.SetUp), message, StackFilter.Filter(stackTrace));
			SkipChildren(_suite, resultState.WithSite(FailureSite.Parent), "OneTimeSetUp: " + message);
		}

		private void SkipChildren(TestSuite suite, ResultState resultState, string message)
		{
			foreach (Test test in suite.Tests)
			{
				if (_childFilter.Pass(test))
				{
					TestResult testResult = test.MakeTestResult();
					testResult.SetResult(resultState, message);
					_suiteResult.AddResult(testResult);
					base.Context.Listener.TestFinished(testResult);
					if (test.IsSuite)
					{
						SkipChildren((TestSuite)test, resultState, message);
					}
				}
			}
		}

		private void PerformOneTimeTearDown()
		{
			base.Context.EstablishExecutionEnvironment();
			_teardownCommand.Execute(base.Context);
		}

		private string GetSkipReason()
		{
			return (string)base.Test.Properties.Get("_SKIPREASON");
		}

		private string GetProviderStackTrace()
		{
			return (string)base.Test.Properties.Get("_PROVIDERSTACKTRACE");
		}

		private void OnChildCompleted(object sender, EventArgs e)
		{
			lock (_completionLock)
			{
				WorkItem workItem = sender as WorkItem;
				if (workItem != null)
				{
					workItem.Completed -= OnChildCompleted;
					_suiteResult.AddResult(workItem.Result);
					if (base.Context.StopOnError && workItem.Result.ResultState.Status == TestStatus.Failed)
					{
						base.Context.ExecutionStatus = TestExecutionStatus.StopRequested;
					}
					CountDownChildTest();
				}
			}
		}

		private void CountDownChildTest()
		{
			_childTestCountdown.Signal();
			if (_childTestCountdown.CurrentCount != 0)
			{
				return;
			}
			if (base.Context.ExecutionStatus != TestExecutionStatus.AbortRequested)
			{
				PerformOneTimeTearDown();
			}
			foreach (ITestResult child in _suiteResult.Children)
			{
				if (child.ResultState == ResultState.Cancelled)
				{
					base.Result.SetResult(ResultState.Cancelled, "Cancelled by user");
					break;
				}
			}
			WorkItemComplete();
		}

		private static bool IsStaticClass(Type type)
		{
			return TypeExtensions.GetTypeInfo(type).IsAbstract && TypeExtensions.GetTypeInfo(type).IsSealed;
		}

		public override void Cancel(bool force)
		{
			lock (cancelLock)
			{
				if (_children == null)
				{
					return;
				}
				foreach (WorkItem child in _children)
				{
					TestExecutionContext context = child.Context;
					if (context != null)
					{
						context.ExecutionStatus = ((!force) ? TestExecutionStatus.StopRequested : TestExecutionStatus.AbortRequested);
					}
					if (child.State == WorkItemState.Running)
					{
						child.Cancel(force);
					}
				}
			}
		}
	}
}
