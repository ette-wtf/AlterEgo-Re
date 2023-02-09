using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class FullNameFilter : ValueMatchFilter
	{
		protected override string ElementName
		{
			get
			{
				return "test";
			}
		}

		public FullNameFilter(string expectedValue)
			: base(expectedValue)
		{
		}

		public override bool Match(ITest test)
		{
			return Match(test.FullName);
		}
	}
}
