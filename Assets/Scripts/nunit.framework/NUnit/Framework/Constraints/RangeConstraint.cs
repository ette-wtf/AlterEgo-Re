using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
	public class RangeConstraint : Constraint
	{
		private readonly IComparable from;

		private readonly IComparable to;

		private ComparisonAdapter comparer = ComparisonAdapter.Default;

		public override string Description
		{
			get
			{
				return string.Format("in range ({0},{1})", from, to);
			}
		}

		public RangeConstraint(IComparable from, IComparable to)
			: base(from, to)
		{
			if (comparer.Compare(from, to) > 0)
			{
				throw new ArgumentException("from must be less than to");
			}
			this.from = from;
			this.to = to;
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			if (from == null || to == null || actual == null)
			{
				throw new ArgumentException("Cannot compare using a null reference", "actual");
			}
			bool isSuccess = comparer.Compare(from, actual) <= 0 && comparer.Compare(to, actual) >= 0;
			return new ConstraintResult(this, actual, isSuccess);
		}

		public RangeConstraint Using(IComparer comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}

		public RangeConstraint Using<T>(IComparer<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}

		public RangeConstraint Using<T>(Comparison<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}
	}
}
