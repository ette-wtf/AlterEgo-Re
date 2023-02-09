namespace NUnit.Framework.Constraints
{
	public abstract class BinaryConstraint : Constraint
	{
		protected IConstraint Left;

		protected IConstraint Right;

		protected BinaryConstraint(IConstraint left, IConstraint right)
			: base(left, right)
		{
			Guard.ArgumentNotNull(left, "left");
			Left = left;
			Guard.ArgumentNotNull(right, "right");
			Right = right;
		}
	}
}
