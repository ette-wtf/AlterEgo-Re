using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class TheoryAttribute : CombiningStrategyAttribute, ITestBuilder, IImplyFixture
	{
		public TheoryAttribute()
			: base(new CombinatorialStrategy(), new ParameterDataProvider(new DatapointProvider(), new ParameterDataSourceProvider()))
		{
		}
	}
}
