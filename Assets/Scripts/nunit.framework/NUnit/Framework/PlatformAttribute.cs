using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class PlatformAttribute : IncludeExcludeAttribute, IApplyToTest
	{
		private PlatformHelper platformHelper = new PlatformHelper();

		public PlatformAttribute()
		{
		}

		public PlatformAttribute(string platforms)
			: base(platforms)
		{
		}

		public void ApplyToTest(Test test)
		{
			if (test.RunState != 0 && test.RunState != RunState.Ignored && !platformHelper.IsPlatformSupported(this))
			{
				test.RunState = RunState.Skipped;
				test.Properties.Add("_SKIPREASON", platformHelper.Reason);
			}
		}
	}
}
