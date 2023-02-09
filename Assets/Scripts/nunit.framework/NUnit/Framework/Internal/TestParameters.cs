using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public abstract class TestParameters : ITestData, IApplyToTest
	{
		public RunState RunState { get; set; }

		public object[] Arguments { get; internal set; }

		public string TestName { get; set; }

		public IPropertyBag Properties { get; private set; }

		public object[] OriginalArguments { get; private set; }

		public TestParameters()
		{
			RunState = RunState.Runnable;
			Properties = new PropertyBag();
		}

		public TestParameters(object[] args)
		{
			RunState = RunState.Runnable;
			InitializeAguments(args);
			Properties = new PropertyBag();
		}

		public TestParameters(Exception exception)
		{
			RunState = RunState.NotRunnable;
			Properties = new PropertyBag();
			Properties.Set("_SKIPREASON", ExceptionHelper.BuildMessage(exception));
			Properties.Set("_PROVIDERSTACKTRACE", ExceptionHelper.BuildStackTrace(exception));
		}

		public TestParameters(ITestData data)
		{
			RunState = data.RunState;
			Properties = new PropertyBag();
			TestName = data.TestName;
			InitializeAguments(data.Arguments);
			foreach (string key in data.Properties.Keys)
			{
				Properties[key] = data.Properties[key];
			}
		}

		private void InitializeAguments(object[] args)
		{
			OriginalArguments = args;
			int num = args.Length;
			Arguments = new object[num];
			Array.Copy(args, Arguments, num);
		}

		public void ApplyToTest(Test test)
		{
			if (RunState != RunState.Runnable)
			{
				test.RunState = RunState;
			}
			foreach (string key in Properties.Keys)
			{
				foreach (object item in Properties[key])
				{
					test.Properties.Add(key, item);
				}
			}
		}
	}
}
