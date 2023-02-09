using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DatapointsAttribute : DatapointSourceAttribute
	{
	}
}
