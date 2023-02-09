using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
	public abstract class CompositeFilter : TestFilter
	{
		public IList<ITestFilter> Filters { get; private set; }

		protected abstract string ElementName { get; }

		public CompositeFilter()
		{
			Filters = new List<ITestFilter>();
		}

		public CompositeFilter(params ITestFilter[] filters)
		{
			Filters = new List<ITestFilter>(filters);
		}

		public void Add(ITestFilter filter)
		{
			Filters.Add(filter);
		}

		public abstract override bool Pass(ITest test);

		public abstract override bool Match(ITest test);

		public abstract override bool IsExplicitMatch(ITest test);

		public override TNode AddToXml(TNode parentNode, bool recursive)
		{
			TNode tNode = parentNode.AddElement(ElementName);
			if (recursive)
			{
				foreach (ITestFilter filter in Filters)
				{
					filter.AddToXml(tNode, true);
				}
			}
			return tNode;
		}
	}
}
