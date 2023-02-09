using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class NotFilter : TestFilter
	{
		public TestFilter BaseFilter { get; private set; }

		public NotFilter(TestFilter baseFilter)
		{
			BaseFilter = baseFilter;
		}

		public override bool Pass(ITest test)
		{
			return !BaseFilter.Match(test) && !BaseFilter.MatchParent(test);
		}

		public override bool Match(ITest test)
		{
			return !BaseFilter.Match(test);
		}

		public override bool IsExplicitMatch(ITest test)
		{
			return false;
		}

		public override TNode AddToXml(TNode parentNode, bool recursive)
		{
			TNode tNode = parentNode.AddElement("not");
			if (recursive)
			{
				BaseFilter.AddToXml(tNode, true);
			}
			return tNode;
		}
	}
}
