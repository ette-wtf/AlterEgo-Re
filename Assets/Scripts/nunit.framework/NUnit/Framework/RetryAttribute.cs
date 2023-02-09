using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class RetryAttribute : PropertyAttribute, IWrapSetUpTearDown, ICommandWrapper
	{
		public class RetryCommand : DelegatingTestCommand
		{
			private int _retryCount;

			public RetryCommand(TestCommand innerCommand, int retryCount)
				: base(innerCommand)
			{
				_retryCount = retryCount;
			}

			public override TestResult Execute(ITestExecutionContext context)
			{
				int retryCount = _retryCount;
				while (retryCount-- > 0)
				{
					context.CurrentResult = innerCommand.Execute(context);
					if (context.CurrentResult.ResultState != ResultState.Failure)
					{
						break;
					}
				}
				return context.CurrentResult;
			}
		}

		private int _count;

		public RetryAttribute(int count)
			: base(count)
		{
			_count = count;
		}

		public TestCommand Wrap(TestCommand command)
		{
			return new RetryCommand(command, _count);
		}
	}
}
