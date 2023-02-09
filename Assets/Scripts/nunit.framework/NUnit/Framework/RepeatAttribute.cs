using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class RepeatAttribute : PropertyAttribute, IWrapSetUpTearDown, ICommandWrapper
	{
		public class RepeatedTestCommand : DelegatingTestCommand
		{
			private int repeatCount;

			public RepeatedTestCommand(TestCommand innerCommand, int repeatCount)
				: base(innerCommand)
			{
				this.repeatCount = repeatCount;
			}

			public override TestResult Execute(ITestExecutionContext context)
			{
				int num = repeatCount;
				while (num-- > 0)
				{
					context.CurrentResult = innerCommand.Execute(context);
					if (context.CurrentResult.ResultState != ResultState.Success)
					{
						break;
					}
				}
				return context.CurrentResult;
			}
		}

		private int _count;

		public RepeatAttribute(int count)
			: base(count)
		{
			_count = count;
		}

		public TestCommand Wrap(TestCommand command)
		{
			return new RepeatedTestCommand(command, _count);
		}
	}
}
