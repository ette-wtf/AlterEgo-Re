namespace NUnit.Framework.Constraints
{
	public class ReusableConstraint : IResolveConstraint
	{
		private readonly IConstraint constraint;

		public ReusableConstraint(IResolveConstraint c)
		{
			constraint = c.Resolve();
		}

		public static implicit operator ReusableConstraint(Constraint c)
		{
			return new ReusableConstraint(c);
		}

		public override string ToString()
		{
			return constraint.ToString();
		}

		public IConstraint Resolve()
		{
			return constraint;
		}
	}
}
