using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
	public abstract class WorkItem
	{
		[Flags]
		private enum OwnThreadReason
		{
			NotNeeded = 0,
			RequiresThread = 1,
			Timeout = 2,
			DifferentApartment = 4
		}

		private static Logger log = InternalTrace.GetLogger("WorkItem");

		private Thread thread;

		private object threadLock = new object();

		public WorkItemState State { get; private set; }

		public Test Test { get; private set; }

		public TestExecutionContext Context { get; private set; }

		public string WorkerId { get; internal set; }

		public List<ITestAction> Actions { get; private set; }

		public TestResult Result { get; protected set; }

		internal ApartmentState TargetApartment { get; set; }

		private ApartmentState CurrentApartment { get; set; }

		public event EventHandler Completed;

		public static WorkItem CreateWorkItem(ITest test, ITestFilter filter)
		{
			TestSuite testSuite = test as TestSuite;
			if (testSuite != null)
			{
				return new CompositeWorkItem(testSuite, filter);
			}
			return new SimpleWorkItem((TestMethod)test, filter);
		}

		public WorkItem(Test test)
		{
			Test = test;
			Result = test.MakeTestResult();
			State = WorkItemState.Ready;
			Actions = new List<ITestAction>();
			TargetApartment = (Test.Properties.ContainsKey("ApartmentState") ? ((ApartmentState)Test.Properties.Get("ApartmentState")) : ApartmentState.Unknown);
		}

		public void InitializeContext(TestExecutionContext context)
		{
			Guard.OperationValid(Context == null, "The context has already been initialized");
			Context = context;
			if (Test is TestAssembly)
			{
				Actions.AddRange(ActionsHelper.GetActionsFromAttributeProvider(((TestAssembly)Test).Assembly));
			}
			else if (Test is ParameterizedMethodSuite)
			{
				Actions.AddRange(ActionsHelper.GetActionsFromAttributeProvider(Test.Method.MethodInfo));
			}
			else if (Test.TypeInfo != null)
			{
				Actions.AddRange(ActionsHelper.GetActionsFromTypesAttributes(Test.TypeInfo.Type));
			}
		}

		public virtual void Execute()
		{
			int num = Context.TestCaseTimeout;
			if (Test.Properties.ContainsKey("Timeout"))
			{
				num = (int)Test.Properties.Get("Timeout");
			}
			OwnThreadReason ownThreadReason = OwnThreadReason.NotNeeded;
			if (Test.RequiresThread)
			{
				ownThreadReason |= OwnThreadReason.RequiresThread;
			}
			if (num > 0 && Test is TestMethod)
			{
				ownThreadReason |= OwnThreadReason.Timeout;
			}
			CurrentApartment = Thread.CurrentThread.GetApartmentState();
			if (CurrentApartment != TargetApartment && TargetApartment != ApartmentState.Unknown)
			{
				ownThreadReason |= OwnThreadReason.DifferentApartment;
			}
			ownThreadReason = OwnThreadReason.NotNeeded;
			if (ownThreadReason == OwnThreadReason.NotNeeded)
			{
				RunTest();
			}
			else if (Context.IsSingleThreaded)
			{
				string message = "Test is not runnable in single-threaded context. " + ownThreadReason;
				log.Error(message);
				Result.SetResult(ResultState.NotRunnable, message);
				WorkItemComplete();
			}
			else
			{
				log.Debug("Running test on own thread. " + ownThreadReason);
				ApartmentState apartment = (((ownThreadReason | OwnThreadReason.DifferentApartment) != 0) ? TargetApartment : CurrentApartment);
				RunTestOnOwnThread(num, apartment);
			}
		}

		private void RunTestOnOwnThread(int timeout, ApartmentState apartment)
		{
			thread = new Thread(RunTest);
			thread.SetApartmentState(apartment);
			RunThread(timeout);
		}

		private void RunThread(int timeout)
		{
			this.thread.CurrentCulture = Context.CurrentCulture;
			this.thread.CurrentUICulture = Context.CurrentUICulture;
			this.thread.Start();
			if (timeout <= 0)
			{
				timeout = -1;
			}
			if (this.thread.Join(timeout))
			{
				return;
			}
			if (Debugger.IsAttached)
			{
				this.thread.Join();
				return;
			}
			Thread thread;
			lock (threadLock)
			{
				if (this.thread == null)
				{
					return;
				}
				thread = this.thread;
				this.thread = null;
			}
			if (Context.ExecutionStatus != TestExecutionStatus.AbortRequested)
			{
				log.Debug("Killing thread {0}, which exceeded timeout", thread.ManagedThreadId);
				ThreadUtility.Kill(thread);
				thread.Join();
				log.Debug("Changing result from {0} to Timeout Failure", Result.ResultState);
				Result.SetResult(ResultState.Failure, string.Format("Test exceeded Timeout value of {0}ms", timeout));
				WorkItemComplete();
			}
		}

		private void RunTest()
		{
			Context.CurrentTest = Test;
			Context.CurrentResult = Result;
			Context.Listener.TestStarted(Test);
			Context.StartTime = DateTime.UtcNow;
			Context.StartTicks = Stopwatch.GetTimestamp();
			Context.WorkerId = WorkerId;
			Context.EstablishExecutionEnvironment();
			State = WorkItemState.Running;
			PerformWork();
		}

		public virtual void Cancel(bool force)
		{
			if (Context != null)
			{
				Context.ExecutionStatus = ((!force) ? TestExecutionStatus.StopRequested : TestExecutionStatus.AbortRequested);
			}
			if (!force)
			{
				return;
			}
			Thread thread;
			lock (threadLock)
			{
				if (this.thread == null)
				{
					return;
				}
				thread = this.thread;
				this.thread = null;
			}
			if (!thread.Join(0))
			{
				log.Debug("Killing thread {0} for cancel", thread.ManagedThreadId);
				ThreadUtility.Kill(thread);
				thread.Join();
				log.Debug("Changing result from {0} to Cancelled", Result.ResultState);
				Result.SetResult(ResultState.Cancelled, "Cancelled by user");
				WorkItemComplete();
			}
		}

		protected abstract void PerformWork();

		protected void WorkItemComplete()
		{
			State = WorkItemState.Complete;
			Result.StartTime = Context.StartTime;
			Result.EndTime = DateTime.UtcNow;
			long num = Stopwatch.GetTimestamp() - Context.StartTicks;
			double duration = (double)num / (double)Stopwatch.Frequency;
			Result.Duration = duration;
			Result.AssertCount += Context.AssertCount;
			Context.Listener.TestFinished(Result);
			if (this.Completed != null)
			{
				this.Completed(this, EventArgs.Empty);
			}
			Context.TestObject = null;
			Test.Fixture = null;
		}
	}
}
