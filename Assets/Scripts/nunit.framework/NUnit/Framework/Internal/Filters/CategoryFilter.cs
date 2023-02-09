using System;
using System.Collections;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class CategoryFilter : ValueMatchFilter
	{
		protected override string ElementName
		{
			get
			{
				return "cat";
			}
		}

		public CategoryFilter(string name)
			: base(name)
		{
		}

		public override bool Match(ITest test)
		{
			IList list = test.Properties["Category"];
			if (list != null)
			{
				foreach (string item in list)
				{
					if (Match(item))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
