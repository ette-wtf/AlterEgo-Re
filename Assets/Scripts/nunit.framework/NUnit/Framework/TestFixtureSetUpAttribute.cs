using System;

namespace NUnit.Framework
{
	[Obsolete("Use OneTimeSetUpAttribute")]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class TestFixtureSetUpAttribute : OneTimeSetUpAttribute
	{
	}
}
