using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class AndFilter : CompositeFilter
	{
		protected override string ElementName
		{
			get
			{
				return "and";
			}
		}

		public AndFilter()
		{
		}

		public AndFilter(params ITestFilter[] filters)
			: base(filters)
		{
		}

		public override bool Pass(ITest test)
		{
			foreach (ITestFilter filter in base.Filters)
			{
				if (!filter.Pass(test))
				{
					return false;
				}
			}
			return true;
		}

		public override bool Match(ITest test)
		{
			foreach (TestFilter filter in base.Filters)
			{
				if (!filter.Match(test))
				{
					return false;
				}
			}
			return true;
		}

		public override bool IsExplicitMatch(ITest test)
		{
			foreach (TestFilter filter in base.Filters)
			{
				if (!filter.IsExplicitMatch(test))
				{
					return false;
				}
			}
			return true;
		}
	}
}
