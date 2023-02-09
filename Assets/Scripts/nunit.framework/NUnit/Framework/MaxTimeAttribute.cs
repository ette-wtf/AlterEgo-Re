using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class MaxTimeAttribute : PropertyAttribute, IWrapSetUpTearDown, ICommandWrapper
	{
		private int _milliseconds;

		public MaxTimeAttribute(int milliseconds)
			: base(milliseconds)
		{
			_milliseconds = milliseconds;
		}

		TestCommand ICommandWrapper.Wrap(TestCommand command)
		{
			return new MaxTimeCommand(command, _milliseconds);
		}
	}
}
