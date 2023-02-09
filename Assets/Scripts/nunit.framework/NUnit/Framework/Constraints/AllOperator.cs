namespace NUnit.Framework.Constraints
{
	public class AllOperator : CollectionOperator
	{
		public override IConstraint ApplyPrefix(IConstraint constraint)
		{
			return new AllItemsConstraint(constraint);
		}
	}
}
