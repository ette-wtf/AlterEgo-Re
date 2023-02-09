namespace NUnit.Framework.Constraints
{
	public class NullConstraint : Constraint
	{
		public NullConstraint()
		{
			Description = "null";
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			NUnitEqualityComparer.CheckGameObjectReference(ref actual);
			return new ConstraintResult(this, actual, actual == null);
		}
	}
}
