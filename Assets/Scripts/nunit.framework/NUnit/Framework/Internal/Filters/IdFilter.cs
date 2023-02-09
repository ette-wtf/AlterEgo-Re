using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class IdFilter : ValueMatchFilter
	{
		protected override string ElementName
		{
			get
			{
				return "id";
			}
		}

		public IdFilter(string id)
			: base(id)
		{
		}

		public override bool Match(ITest test)
		{
			return test.Id == base.ExpectedValue;
		}
	}
}
