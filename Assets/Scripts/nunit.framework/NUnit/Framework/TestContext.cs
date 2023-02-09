using System;
using System.IO;
using System.Reflection;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework
{
	public class TestContext
	{
		public class TestAdapter
		{
			private readonly Test _test;

			public string ID
			{
				get
				{
					return _test.Id;
				}
			}

			public string Name
			{
				get
				{
					return _test.Name;
				}
			}

			public string MethodName
			{
				get
				{
					return (_test is TestMethod) ? _test.Method.Name : null;
				}
			}

			public string FullName
			{
				get
				{
					return _test.FullName;
				}
			}

			public string ClassName
			{
				get
				{
					return _test.ClassName;
				}
			}

			public IPropertyBag Properties
			{
				get
				{
					return _test.Properties;
				}
			}

			public TestAdapter(Test test)
			{
				_test = test;
			}
		}

		public class ResultAdapter
		{
			private readonly TestResult _result;

			public ResultState Outcome
			{
				get
				{
					return _result.ResultState;
				}
			}

			public string Message
			{
				get
				{
					return _result.Message;
				}
			}

			public virtual string StackTrace
			{
				get
				{
					return _result.StackTrace;
				}
			}

			public int FailCount
			{
				get
				{
					return _result.FailCount;
				}
			}

			public int PassCount
			{
				get
				{
					return _result.PassCount;
				}
			}

			public int SkipCount
			{
				get
				{
					return _result.SkipCount;
				}
			}

			public int InconclusiveCount
			{
				get
				{
					return _result.InconclusiveCount;
				}
			}

			public ResultAdapter(TestResult result)
			{
				_result = result;
			}
		}

		public static ITestExecutionContext CurrentTestExecutionContext;

		private TestAdapter _test;

		private ResultAdapter _result;

		public static TextWriter Error = new EventListenerTextWriter("Error", Console.Error);

		public static readonly TextWriter Progress = new EventListenerTextWriter("Progress", Console.Error);

		public static readonly TestParameters Parameters = new TestParameters();

		private ITestExecutionContext _testExecutionContext;

		public static TestContext CurrentContext
		{
			get
			{
				return new TestContext(CurrentTestExecutionContext ?? TestExecutionContext.CurrentContext);
			}
		}

		public static TextWriter Out
		{
			get
			{
				return (CurrentTestExecutionContext ?? TestExecutionContext.CurrentContext).OutWriter;
			}
		}

		public TestAdapter Test
		{
			get
			{
				return _test ?? (_test = new TestAdapter(_testExecutionContext.CurrentTest));
			}
		}

		public ResultAdapter Result
		{
			get
			{
				return _result ?? (_result = new ResultAdapter(_testExecutionContext.CurrentResult));
			}
		}

		public string WorkerId
		{
			get
			{
				return _testExecutionContext.WorkerId;
			}
		}

		public string TestDirectory
		{
			get
			{
				Test currentTest = _testExecutionContext.CurrentTest;
				if (currentTest != null)
				{
					return AssemblyHelper.GetDirectoryName(currentTest.TypeInfo.Assembly);
				}
				return AssemblyHelper.GetDirectoryName(Assembly.GetCallingAssembly());
			}
		}

		public string WorkDirectory
		{
			get
			{
				return _testExecutionContext.WorkDirectory;
			}
		}

		public Randomizer Random
		{
			get
			{
				return _testExecutionContext.RandomGenerator;
			}
		}

		public TestContext(ITestExecutionContext testExecutionContext)
		{
			_testExecutionContext = testExecutionContext;
		}

		public static void Write(bool value)
		{
			Out.Write(value);
		}

		public static void Write(char value)
		{
			Out.Write(value);
		}

		public static void Write(char[] value)
		{
			Out.Write(value);
		}

		public static void Write(double value)
		{
			Out.Write(value);
		}

		public static void Write(int value)
		{
			Out.Write(value);
		}

		public static void Write(long value)
		{
			Out.Write(value);
		}

		public static void Write(decimal value)
		{
			Out.Write(value);
		}

		public static void Write(object value)
		{
			Out.Write(value);
		}

		public static void Write(float value)
		{
			Out.Write(value);
		}

		public static void Write(string value)
		{
			Out.Write(value);
		}

		[CLSCompliant(false)]
		public static void Write(uint value)
		{
			Out.Write(value);
		}

		[CLSCompliant(false)]
		public static void Write(ulong value)
		{
			Out.Write(value);
		}

		public static void Write(string format, object arg1)
		{
			Out.Write(format, arg1);
		}

		public static void Write(string format, object arg1, object arg2)
		{
			Out.Write(format, arg1, arg2);
		}

		public static void Write(string format, object arg1, object arg2, object arg3)
		{
			Out.Write(format, arg1, arg2, arg3);
		}

		public static void Write(string format, params object[] args)
		{
			Out.Write(format, args);
		}

		public static void WriteLine()
		{
			Out.WriteLine();
		}

		public static void WriteLine(bool value)
		{
			Out.WriteLine(value);
		}

		public static void WriteLine(char value)
		{
			Out.WriteLine(value);
		}

		public static void WriteLine(char[] value)
		{
			Out.WriteLine(value);
		}

		public static void WriteLine(double value)
		{
			Out.WriteLine(value);
		}

		public static void WriteLine(int value)
		{
			Out.WriteLine(value);
		}

		public static void WriteLine(long value)
		{
			Out.WriteLine(value);
		}

		public static void WriteLine(decimal value)
		{
			Out.WriteLine(value);
		}

		public static void WriteLine(object value)
		{
			Out.WriteLine(value);
		}

		public static void WriteLine(float value)
		{
			Out.WriteLine(value);
		}

		public static void WriteLine(string value)
		{
			Out.WriteLine(value);
		}

		[CLSCompliant(false)]
		public static void WriteLine(uint value)
		{
			Out.WriteLine(value);
		}

		[CLSCompliant(false)]
		public static void WriteLine(ulong value)
		{
			Out.WriteLine(value);
		}

		public static void WriteLine(string format, object arg1)
		{
			Out.WriteLine(format, arg1);
		}

		public static void WriteLine(string format, object arg1, object arg2)
		{
			Out.WriteLine(format, arg1, arg2);
		}

		public static void WriteLine(string format, object arg1, object arg2, object arg3)
		{
			Out.WriteLine(format, arg1, arg2, arg3);
		}

		public static void WriteLine(string format, params object[] args)
		{
			Out.WriteLine(format, args);
		}

		public static void AddFormatter(ValueFormatterFactory formatterFactory)
		{
			TestExecutionContext.CurrentContext.AddFormatter(formatterFactory);
		}

		public static void AddFormatter<TSUPPORTED>(ValueFormatter formatter)
		{
			AddFormatter((ValueFormatter next) => (object val) => (val is TSUPPORTED) ? formatter(val) : next(val));
		}
	}
}
