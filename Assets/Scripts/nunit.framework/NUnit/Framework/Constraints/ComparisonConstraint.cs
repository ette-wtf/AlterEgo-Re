using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
	public abstract class ComparisonConstraint : Constraint
	{
		protected object expected;

		protected bool lessComparisonResult = false;

		protected bool equalComparisonResult = false;

		protected bool greaterComparisonResult = false;

		private ComparisonAdapter comparer = ComparisonAdapter.Default;

		protected ComparisonConstraint(object value, bool lessComparisonResult, bool equalComparisonResult, bool greaterComparisonResult, string predicate)
			: base(value)
		{
			expected = value;
			this.lessComparisonResult = lessComparisonResult;
			this.equalComparisonResult = equalComparisonResult;
			this.greaterComparisonResult = greaterComparisonResult;
			Description = predicate + " " + MsgUtils.FormatValue(expected);
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			if (expected == null)
			{
				throw new ArgumentException("Cannot compare using a null reference", "expected");
			}
			if (actual == null)
			{
				throw new ArgumentException("Cannot compare to null reference", "actual");
			}
			int num = comparer.Compare(expected, actual);
			bool isSuccess = (num < 0 && greaterComparisonResult) || (num == 0 && equalComparisonResult) || (num > 0 && lessComparisonResult);
			return new ConstraintResult(this, actual, isSuccess);
		}

		public ComparisonConstraint Using(IComparer comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}

		public ComparisonConstraint Using<T>(IComparer<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}

		public ComparisonConstraint Using<T>(Comparison<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}
	}
}
