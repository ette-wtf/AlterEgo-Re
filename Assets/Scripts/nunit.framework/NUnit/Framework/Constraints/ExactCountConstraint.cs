using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class ExactCountConstraint : PrefixConstraint
	{
		private int expectedCount;

		public ExactCountConstraint(int expectedCount, IConstraint itemConstraint)
			: base(itemConstraint)
		{
			this.expectedCount = expectedCount;
			object descriptionPrefix;
			switch (expectedCount)
			{
			default:
				descriptionPrefix = string.Format("exactly {0} items", expectedCount);
				break;
			case 1:
				descriptionPrefix = "exactly one item";
				break;
			case 0:
				descriptionPrefix = "no item";
				break;
			}
			base.DescriptionPrefix = (string)descriptionPrefix;
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			if (!(actual is IEnumerable))
			{
				throw new ArgumentException("The actual value must be an IEnumerable", "actual");
			}
			int num = 0;
			foreach (object item in (IEnumerable)actual)
			{
				if (base.BaseConstraint.ApplyTo(item).IsSuccess)
				{
					num++;
				}
			}
			return new ConstraintResult(this, actual, num == expectedCount);
		}
	}
}
