using System;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class SequentialAttribute : CombiningStrategyAttribute
	{
		public SequentialAttribute()
			: base(new SequentialStrategy(), new ParameterDataSourceProvider())
		{
		}
	}
}
