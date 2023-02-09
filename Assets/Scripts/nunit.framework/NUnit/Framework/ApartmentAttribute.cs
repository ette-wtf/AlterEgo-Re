using System;
using System.Threading;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class ApartmentAttribute : PropertyAttribute
	{
		public ApartmentAttribute(ApartmentState apartmentState)
		{
			Guard.ArgumentValid(apartmentState != ApartmentState.Unknown, "must be STA or MTA", "apartmentState");
			base.Properties.Add("ApartmentState", apartmentState);
		}
	}
}
