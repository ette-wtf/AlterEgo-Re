using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class OrderAttribute : NUnitAttribute, IApplyToTest
	{
		public readonly int Order;

		public OrderAttribute(int order)
		{
			Order = order;
		}

		public void ApplyToTest(Test test)
		{
			if (!test.Properties.ContainsKey("Order"))
			{
				test.Properties.Set("Order", Order);
			}
		}
	}
}
