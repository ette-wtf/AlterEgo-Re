using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class CollectionContainsConstraint : CollectionItemsEqualConstraint
	{
		public override string DisplayName
		{
			get
			{
				return "Contains";
			}
		}

		public override string Description
		{
			get
			{
				return "collection containing " + MsgUtils.FormatValue(Expected);
			}
		}

		protected object Expected { get; private set; }

		public CollectionContainsConstraint(object expected)
			: base(expected)
		{
			Expected = expected;
		}

		protected override bool Matches(IEnumerable actual)
		{
			foreach (object item in actual)
			{
				if (ItemsEqual(item, Expected))
				{
					return true;
				}
			}
			return false;
		}

		public CollectionContainsConstraint Using<TCollectionType, TMemberType>(Func<TCollectionType, TMemberType, bool> comparison)
		{
			Func<TMemberType, TCollectionType, bool> comparison2 = (TMemberType actual, TCollectionType expected) => comparison(expected, actual);
			Using(EqualityAdapter.For(comparison2));
			return this;
		}
	}
}
