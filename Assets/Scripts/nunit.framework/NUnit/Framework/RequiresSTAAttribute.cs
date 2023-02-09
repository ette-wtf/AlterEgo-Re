using System;
using System.Threading;

namespace NUnit.Framework
{
	[Obsolete("Use ApartmentAttribute and pass in ApartmentState.STA instead")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class RequiresSTAAttribute : PropertyAttribute
	{
		public RequiresSTAAttribute()
		{
			base.Properties.Add("ApartmentState", ApartmentState.STA);
		}
	}
}
