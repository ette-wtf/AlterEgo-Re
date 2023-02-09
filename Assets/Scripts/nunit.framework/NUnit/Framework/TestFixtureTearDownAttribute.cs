using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	[Obsolete("Use OneTimeTearDownAttribute")]
	public class TestFixtureTearDownAttribute : OneTimeTearDownAttribute
	{
	}
}
