using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Principal;
using System.Threading;
using NUnit.Compatibility;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework.Internal
{
	public class TestExecutionContext : LongLivedMarshalByRefObject, ITestExecutionContext
	{
		private TestExecutionContext _priorContext;

		private TestExecutionStatus _executionStatus;

		private ITestListener _listener = TestListener.NULL;

		private int _assertCount;

		private Randomizer _randomGenerator;

		private CultureInfo _currentCulture;

		private CultureInfo _currentUICulture;

		private TestResult _currentResult;

		private IPrincipal _currentPrincipal;

		private static readonly string CONTEXT_KEY = "NUnit.Framework.TestContext";

		public static ITestExecutionContext CurrentContext
		{
			[SecuritySafeCritical]
			get
			{
				TestExecutionContext testExecutionContext = GetTestExecutionContext();
				if (testExecutionContext == null)
				{
					testExecutionContext = new TestExecutionContext();
					CallContext.SetData(CONTEXT_KEY, testExecutionContext);
				}
				return testExecutionContext;
			}
			[SecuritySafeCritical]
			private set
			{
				if (value == null)
				{
					CallContext.FreeNamedDataSlot(CONTEXT_KEY);
				}
				else
				{
					CallContext.SetData(CONTEXT_KEY, value);
				}
			}
		}

		public Test CurrentTest { get; set; }

		public DateTime StartTime { get; set; }

		public long StartTicks { get; set; }

		public TestResult CurrentResult
		{
			get
			{
				return _currentResult;
			}
			set
			{
				_currentResult = value;
				if (value != null)
				{
					OutWriter = value.OutWriter;
				}
			}
		}

		public TextWriter OutWriter { get; private set; }

		public object TestObject { get; set; }

		public string WorkDirectory { get; set; }

		public bool StopOnError { get; set; }

		public TestExecutionStatus ExecutionStatus
		{
			get
			{
				if (_executionStatus == TestExecutionStatus.Running && _priorContext != null)
				{
					_executionStatus = _priorContext.ExecutionStatus;
				}
				return _executionStatus;
			}
			set
			{
				_executionStatus = value;
				if (_priorContext != null)
				{
					_priorContext.ExecutionStatus = value;
				}
			}
		}

		internal ITestListener Listener
		{
			get
			{
				return _listener;
			}
			set
			{
				_listener = value;
			}
		}

		public IWorkItemDispatcher Dispatcher { get; set; }

		public ParallelScope ParallelScope { get; set; }

		public string WorkerId { get; internal set; }

		public Randomizer RandomGenerator
		{
			get
			{
				if (_randomGenerator == null)
				{
					_randomGenerator = new Randomizer(CurrentTest.Seed);
				}
				return _randomGenerator;
			}
		}

		internal int AssertCount
		{
			get
			{
				return _assertCount;
			}
		}

		public int TestCaseTimeout { get; set; }

		public List<ITestAction> UpstreamActions { get; private set; }

		public CultureInfo CurrentCulture
		{
			get
			{
				return _currentCulture;
			}
			set
			{
				_currentCulture = value;
				Thread.CurrentThread.CurrentCulture = _currentCulture;
			}
		}

		public CultureInfo CurrentUICulture
		{
			get
			{
				return _currentUICulture;
			}
			set
			{
				_currentUICulture = value;
				Thread.CurrentThread.CurrentUICulture = _currentUICulture;
			}
		}

		public IPrincipal CurrentPrincipal
		{
			get
			{
				return _currentPrincipal;
			}
			set
			{
				_currentPrincipal = value;
				Thread.CurrentPrincipal = _currentPrincipal;
			}
		}

		public ValueFormatter CurrentValueFormatter { get; private set; }

		public bool IsSingleThreaded { get; set; }

		public TestExecutionContext()
		{
			_priorContext = null;
			TestCaseTimeout = 0;
			UpstreamActions = new List<ITestAction>();
			_currentCulture = CultureInfo.CurrentCulture;
			_currentUICulture = CultureInfo.CurrentUICulture;
			_currentPrincipal = Thread.CurrentPrincipal;
			CurrentValueFormatter = (object val) => MsgUtils.DefaultValueFormatter(val);
			IsSingleThreaded = false;
		}

		public TestExecutionContext(TestExecutionContext other)
		{
			_priorContext = other;
			CurrentTest = other.CurrentTest;
			CurrentResult = other.CurrentResult;
			TestObject = other.TestObject;
			WorkDirectory = other.WorkDirectory;
			_listener = other._listener;
			StopOnError = other.StopOnError;
			TestCaseTimeout = other.TestCaseTimeout;
			UpstreamActions = new List<ITestAction>(other.UpstreamActions);
			_currentCulture = other.CurrentCulture;
			_currentUICulture = other.CurrentUICulture;
			_currentPrincipal = other.CurrentPrincipal;
			CurrentValueFormatter = other.CurrentValueFormatter;
			Dispatcher = other.Dispatcher;
			ParallelScope = other.ParallelScope;
			IsSingleThreaded = other.IsSingleThreaded;
		}

		[SecuritySafeCritical]
		public static TestExecutionContext GetTestExecutionContext()
		{
			return CallContext.GetData(CONTEXT_KEY) as TestExecutionContext;
		}

		public static void ClearCurrentContext()
		{
			CurrentContext = null;
		}

		public void UpdateContextFromEnvironment()
		{
			_currentCulture = CultureInfo.CurrentCulture;
			_currentUICulture = CultureInfo.CurrentUICulture;
			_currentPrincipal = Thread.CurrentPrincipal;
		}

		public void EstablishExecutionEnvironment()
		{
			Thread.CurrentThread.CurrentCulture = _currentCulture;
			Thread.CurrentThread.CurrentUICulture = _currentUICulture;
			Thread.CurrentPrincipal = _currentPrincipal;
			CurrentContext = this;
		}

		public void IncrementAssertCount()
		{
			Interlocked.Increment(ref _assertCount);
		}

		public void IncrementAssertCount(int count)
		{
			while (count-- > 0)
			{
				Interlocked.Increment(ref _assertCount);
			}
		}

		public void AddFormatter(ValueFormatterFactory formatterFactory)
		{
			CurrentValueFormatter = formatterFactory(CurrentValueFormatter);
		}

		[SecurityCritical]
		public override object InitializeLifetimeService()
		{
			return null;
		}
	}
}
