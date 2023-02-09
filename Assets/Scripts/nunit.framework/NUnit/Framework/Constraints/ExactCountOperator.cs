namespace NUnit.Framework.Constraints
{
	public class ExactCountOperator : CollectionOperator
	{
		private int expectedCount;

		public ExactCountOperator(int expectedCount)
		{
			this.expectedCount = expectedCount;
		}

		public override IConstraint ApplyPrefix(IConstraint constraint)
		{
			return new ExactCountConstraint(expectedCount, constraint);
		}
	}
}
