using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	public abstract class TestActionAttribute : Attribute, ITestAction
	{
		public virtual ActionTargets Targets
		{
			get
			{
				return ActionTargets.Default;
			}
		}

		public virtual void BeforeTest(ITest test)
		{
		}

		public virtual void AfterTest(ITest test)
		{
		}
	}
}
