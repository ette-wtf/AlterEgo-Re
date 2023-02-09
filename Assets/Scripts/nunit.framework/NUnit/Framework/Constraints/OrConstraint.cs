namespace NUnit.Framework.Constraints
{
	public class OrConstraint : BinaryConstraint
	{
		public override string Description
		{
			get
			{
				return Left.Description + " or " + Right.Description;
			}
		}

		public OrConstraint(IConstraint left, IConstraint right)
			: base(left, right)
		{
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			bool isSuccess = Left.ApplyTo(actual).IsSuccess || Right.ApplyTo(actual).IsSuccess;
			return new ConstraintResult(this, actual, isSuccess);
		}
	}
}
