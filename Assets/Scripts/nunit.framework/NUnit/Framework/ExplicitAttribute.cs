using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ExplicitAttribute : NUnitAttribute, IApplyToTest
	{
		private string _reason;

		public ExplicitAttribute()
		{
		}

		public ExplicitAttribute(string reason)
		{
			_reason = reason;
		}

		public void ApplyToTest(Test test)
		{
			if (test.RunState != 0 && test.RunState != RunState.Ignored)
			{
				test.RunState = RunState.Explicit;
				if (_reason != null)
				{
					test.Properties.Set("_SKIPREASON", _reason);
				}
			}
		}
	}
}
