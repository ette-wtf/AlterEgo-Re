using System;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class RequiresThreadAttribute : PropertyAttribute, IApplyToTest
	{
		public RequiresThreadAttribute()
			: base(true)
		{
		}

		public RequiresThreadAttribute(ApartmentState apartment)
			: base(true)
		{
			base.Properties.Add("ApartmentState", apartment);
		}

		void IApplyToTest.ApplyToTest(Test test)
		{
			test.RequiresThread = true;
			base.ApplyToTest(test);
		}
	}
}
