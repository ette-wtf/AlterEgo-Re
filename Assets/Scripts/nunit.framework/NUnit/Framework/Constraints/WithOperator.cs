namespace NUnit.Framework.Constraints
{
	public class WithOperator : PrefixOperator
	{
		public WithOperator()
		{
			left_precedence = 1;
			right_precedence = 4;
		}

		public override IConstraint ApplyPrefix(IConstraint constraint)
		{
			return constraint;
		}
	}
}
