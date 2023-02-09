using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class CollectionSupersetConstraint : CollectionItemsEqualConstraint
	{
		private IEnumerable _expected;

		public override string DisplayName
		{
			get
			{
				return "SupersetOf";
			}
		}

		public override string Description
		{
			get
			{
				return "superset of " + MsgUtils.FormatValue(_expected);
			}
		}

		public CollectionSupersetConstraint(IEnumerable expected)
			: base(expected)
		{
			_expected = expected;
		}

		protected override bool Matches(IEnumerable actual)
		{
			return Tally(actual).TryRemove(_expected);
		}

		public CollectionSupersetConstraint Using<TSupersetType, TSubsetType>(Func<TSupersetType, TSubsetType, bool> comparison)
		{
			Func<TSubsetType, TSupersetType, bool> comparison2 = (TSubsetType actual, TSupersetType expected) => comparison(expected, actual);
			Using(EqualityAdapter.For(comparison2));
			return this;
		}
	}
}
