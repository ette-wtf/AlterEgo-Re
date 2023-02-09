using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class CollectionEquivalentConstraint : CollectionItemsEqualConstraint
	{
		private readonly IEnumerable _expected;

		public override string DisplayName
		{
			get
			{
				return "Equivalent";
			}
		}

		public override string Description
		{
			get
			{
				return "equivalent to " + MsgUtils.FormatValue(_expected);
			}
		}

		public CollectionEquivalentConstraint(IEnumerable expected)
			: base(expected)
		{
			_expected = expected;
		}

		protected override bool Matches(IEnumerable actual)
		{
			if (_expected is ICollection && actual is ICollection && ((ICollection)actual).Count != ((ICollection)_expected).Count)
			{
				return false;
			}
			CollectionTally collectionTally = Tally(_expected);
			return collectionTally.TryRemove(actual) && collectionTally.Count == 0;
		}

		public CollectionEquivalentConstraint Using<TActual, TExpected>(Func<TActual, TExpected, bool> comparison)
		{
			Using(EqualityAdapter.For(comparison));
			return this;
		}
	}
}
