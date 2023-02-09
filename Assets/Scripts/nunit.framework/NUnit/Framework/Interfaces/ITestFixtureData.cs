using System;

namespace NUnit.Framework.Interfaces
{
	public interface ITestFixtureData : ITestData
	{
		Type[] TypeArgs { get; }
	}
}
