using System;
using System.Threading;

namespace NUnit.Framework
{
	[Obsolete("Use ApartmentAttribute and pass in ApartmentState.MTA instead")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class RequiresMTAAttribute : PropertyAttribute
	{
		public RequiresMTAAttribute()
		{
			base.Properties.Add("ApartmentState", ApartmentState.MTA);
		}
	}
}
