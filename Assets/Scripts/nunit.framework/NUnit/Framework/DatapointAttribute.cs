using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DatapointAttribute : NUnitAttribute
	{
	}
}
