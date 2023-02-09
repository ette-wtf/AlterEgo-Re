using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class MethodNameFilter : ValueMatchFilter
	{
		protected override string ElementName
		{
			get
			{
				return "method";
			}
		}

		public MethodNameFilter(string expectedValue)
			: base(expectedValue)
		{
		}

		public override bool Match(ITest test)
		{
			return Match(test.MethodName);
		}
	}
}
