namespace NUnit.Framework.Constraints
{
	public class TrueConstraint : Constraint
	{
		public TrueConstraint()
		{
			Description = "True";
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			return new ConstraintResult(this, actual, true.Equals(actual));
		}
	}
}
