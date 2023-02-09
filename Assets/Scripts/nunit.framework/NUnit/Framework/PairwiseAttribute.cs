using System;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class PairwiseAttribute : CombiningStrategyAttribute
	{
		public PairwiseAttribute()
			: base(new PairwiseStrategy(), new ParameterDataSourceProvider())
		{
		}
	}
}
