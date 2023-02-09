using System;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class CombinatorialAttribute : CombiningStrategyAttribute
	{
		public CombinatorialAttribute()
			: base(new CombinatorialStrategy(), new ParameterDataSourceProvider())
		{
		}
	}
}
