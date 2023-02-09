using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class TestFixtureParameters : TestParameters, ITestFixtureData, ITestData
	{
		public Type[] TypeArgs { get; internal set; }

		public TestFixtureParameters()
		{
		}

		public TestFixtureParameters(Exception exception)
			: base(exception)
		{
		}

		public TestFixtureParameters(params object[] args)
			: base(args)
		{
		}

		public TestFixtureParameters(ITestFixtureData data)
			: base(data)
		{
			TypeArgs = data.TypeArgs;
		}
	}
}
