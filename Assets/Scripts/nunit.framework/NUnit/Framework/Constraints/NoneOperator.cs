namespace NUnit.Framework.Constraints
{
	public class NoneOperator : CollectionOperator
	{
		public override IConstraint ApplyPrefix(IConstraint constraint)
		{
			return new NoItemConstraint(constraint);
		}
	}
}
