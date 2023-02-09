using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
	public class UniqueItemsConstraint : CollectionItemsEqualConstraint
	{
		public override string Description
		{
			get
			{
				return "all items unique";
			}
		}

		protected override bool Matches(IEnumerable actual)
		{
			List<object> list = new List<object>();
			foreach (object item in actual)
			{
				foreach (object item2 in list)
				{
					if (ItemsEqual(item, item2))
					{
						return false;
					}
				}
				list.Add(item);
			}
			return true;
		}
	}
}
