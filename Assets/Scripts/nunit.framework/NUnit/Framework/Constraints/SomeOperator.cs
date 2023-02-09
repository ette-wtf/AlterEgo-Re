namespace NUnit.Framework.Constraints
{
	public class SomeOperator : CollectionOperator
	{
		public override IConstraint ApplyPrefix(IConstraint constraint)
		{
			return new SomeItemsConstraint(constraint);
		}
	}
}
