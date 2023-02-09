using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
	public class OneTimeSetUpCommand : TestCommand
	{
		private readonly TestSuite _suite;

		private readonly ITypeInfo _typeInfo;

		private readonly object[] _arguments;

		private readonly List<SetUpTearDownItem> _setUpTearDown;

		private readonly List<TestActionItem> _actions;

		public OneTimeSetUpCommand(TestSuite suite, List<SetUpTearDownItem> setUpTearDown, List<TestActionItem> actions)
			: base(suite)
		{
			_suite = suite;
			_typeInfo = suite.TypeInfo;
			_arguments = suite.Arguments;
			_setUpTearDown = setUpTearDown;
			_actions = actions;
		}

		public override TestResult Execute(ITestExecutionContext context)
		{
			if (_typeInfo != null)
			{
				if (!_typeInfo.IsStaticClass)
				{
					context.TestObject = _suite.Fixture ?? _typeInfo.Construct(_arguments);
					if (_suite.Fixture == null)
					{
						_suite.Fixture = context.TestObject;
					}
					base.Test.Fixture = _suite.Fixture;
				}
				int num = _setUpTearDown.Count;
				while (num > 0)
				{
					_setUpTearDown[--num].RunSetUp(context);
				}
			}
			for (int num = 0; num < _actions.Count; num++)
			{
				_actions[num].BeforeTest(base.Test);
			}
			return context.CurrentResult;
		}
	}
}
