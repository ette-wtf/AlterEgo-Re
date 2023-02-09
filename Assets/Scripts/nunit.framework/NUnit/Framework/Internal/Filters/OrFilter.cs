using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class OrFilter : CompositeFilter
	{
		protected override string ElementName
		{
			get
			{
				return "or";
			}
		}

		public OrFilter()
		{
		}

		public OrFilter(params ITestFilter[] filters)
			: base(filters)
		{
		}

		public override bool Pass(ITest test)
		{
			foreach (ITestFilter filter in base.Filters)
			{
				if (filter.Pass(test))
				{
					return true;
				}
			}
			return false;
		}

		public override bool Match(ITest test)
		{
			foreach (TestFilter filter in base.Filters)
			{
				if (filter.Match(test))
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsExplicitMatch(ITest test)
		{
			foreach (TestFilter filter in base.Filters)
			{
				if (filter.IsExplicitMatch(test))
				{
					return true;
				}
			}
			return false;
		}
	}
}
