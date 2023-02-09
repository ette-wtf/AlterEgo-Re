using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class AllItemsConstraint : PrefixConstraint
	{
		public override string DisplayName
		{
			get
			{
				return "All";
			}
		}

		public AllItemsConstraint(IConstraint itemConstraint)
			: base(itemConstraint)
		{
			base.DescriptionPrefix = "all items";
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			if (!(actual is IEnumerable))
			{
				throw new ArgumentException("The actual value must be an IEnumerable", "actual");
			}
			foreach (object item in (IEnumerable)actual)
			{
				if (!base.BaseConstraint.ApplyTo(item).IsSuccess)
				{
					return new ConstraintResult(this, actual, ConstraintStatus.Failure);
				}
			}
			return new ConstraintResult(this, actual, ConstraintStatus.Success);
		}
	}
}
