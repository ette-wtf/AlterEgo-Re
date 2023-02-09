namespace NUnit.Framework.Constraints
{
	public class NotConstraint : PrefixConstraint
	{
		public NotConstraint(IConstraint baseConstraint)
			: base(baseConstraint)
		{
			base.DescriptionPrefix = "not";
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			ConstraintResult constraintResult = base.BaseConstraint.ApplyTo(actual);
			return new ConstraintResult(this, constraintResult.ActualValue, !constraintResult.IsSuccess);
		}
	}
}
