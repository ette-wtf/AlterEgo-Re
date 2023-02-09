using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class ParallelizableAttribute : PropertyAttribute, IApplyToContext
	{
		private ParallelScope _scope;

		public ParallelizableAttribute()
			: this(ParallelScope.Self)
		{
		}

		public ParallelizableAttribute(ParallelScope scope)
		{
			_scope = scope;
			base.Properties.Add("ParallelScope", scope);
		}

		public void ApplyToContext(ITestExecutionContext context)
		{
			context.ParallelScope = _scope & ~ParallelScope.Self;
		}
	}
}
