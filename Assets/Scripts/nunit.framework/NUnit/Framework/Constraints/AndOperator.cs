namespace NUnit.Framework.Constraints
{
	public class AndOperator : BinaryOperator
	{
		public AndOperator()
		{
			left_precedence = (right_precedence = 2);
		}

		public override IConstraint ApplyOperator(IConstraint left, IConstraint right)
		{
			return new AndConstraint(left, right);
		}
	}
}
