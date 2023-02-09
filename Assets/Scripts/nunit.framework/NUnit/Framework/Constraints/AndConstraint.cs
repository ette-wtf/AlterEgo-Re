namespace NUnit.Framework.Constraints
{
	public class AndConstraint : BinaryConstraint
	{
		private class AndConstraintResult : ConstraintResult
		{
			private ConstraintResult leftResult;

			private ConstraintResult rightResult;

			public AndConstraintResult(AndConstraint constraint, object actual, ConstraintResult leftResult, ConstraintResult rightResult)
				: base(constraint, actual, leftResult.IsSuccess && rightResult.IsSuccess)
			{
				this.leftResult = leftResult;
				this.rightResult = rightResult;
			}

			public override void WriteActualValueTo(MessageWriter writer)
			{
				if (IsSuccess)
				{
					base.WriteActualValueTo(writer);
				}
				else if (!leftResult.IsSuccess)
				{
					leftResult.WriteActualValueTo(writer);
				}
				else
				{
					rightResult.WriteActualValueTo(writer);
				}
			}
		}

		public override string Description
		{
			get
			{
				return Left.Description + " and " + Right.Description;
			}
		}

		public AndConstraint(IConstraint left, IConstraint right)
			: base(left, right)
		{
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			ConstraintResult constraintResult = Left.ApplyTo(actual);
			ConstraintResult rightResult = (constraintResult.IsSuccess ? Right.ApplyTo(actual) : new ConstraintResult(Right, actual));
			return new AndConstraintResult(this, actual, constraintResult, rightResult);
		}
	}
}
