using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class CollectionSubsetConstraint : CollectionItemsEqualConstraint
	{
		private IEnumerable _expected;

		public override string DisplayName
		{
			get
			{
				return "SubsetOf";
			}
		}

		public override string Description
		{
			get
			{
				return "subset of " + MsgUtils.FormatValue(_expected);
			}
		}

		public CollectionSubsetConstraint(IEnumerable expected)
			: base(expected)
		{
			_expected = expected;
		}

		protected override bool Matches(IEnumerable actual)
		{
			return Tally(_expected).TryRemove(actual);
		}

		public CollectionSubsetConstraint Using<TSubsetType, TSupersetType>(Func<TSubsetType, TSupersetType, bool> comparison)
		{
			Using(EqualityAdapter.For(comparison));
			return this;
		}
	}
}
