using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
	public class TestActionItem
	{
		private readonly ITestAction _action;

		private bool _beforeTestWasRun;

		public TestActionItem(ITestAction action)
		{
			_action = action;
		}

		public void BeforeTest(ITest test)
		{
			_beforeTestWasRun = true;
			_action.BeforeTest(test);
		}

		public void AfterTest(ITest test)
		{
			if (_beforeTestWasRun)
			{
				_action.AfterTest(test);
			}
		}
	}
}
