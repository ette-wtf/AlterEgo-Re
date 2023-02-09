using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class TestNameFilter : ValueMatchFilter
	{
		protected override string ElementName
		{
			get
			{
				return "name";
			}
		}

		public TestNameFilter(string expectedValue)
			: base(expectedValue)
		{
		}

		public override bool Match(ITest test)
		{
			return Match(test.Name);
		}
	}
}
