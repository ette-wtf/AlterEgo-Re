namespace NUnit.Framework.Constraints
{
	public class NaNConstraint : Constraint
	{
		public override string Description
		{
			get
			{
				return "NaN";
			}
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			return new ConstraintResult(this, actual, (actual is double && double.IsNaN((double)actual)) || (actual is float && float.IsNaN((float)actual)));
		}
	}
}
