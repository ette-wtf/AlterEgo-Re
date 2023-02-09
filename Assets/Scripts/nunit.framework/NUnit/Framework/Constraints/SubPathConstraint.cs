namespace NUnit.Framework.Constraints
{
	public class SubPathConstraint : PathConstraint
	{
		public override string Description
		{
			get
			{
				return "Subpath of " + MsgUtils.FormatValue(expected);
			}
		}

		public SubPathConstraint(string expected)
			: base(expected)
		{
		}

		protected override bool Matches(string actual)
		{
			return actual != null && IsSubPath(Canonicalize(expected), Canonicalize(actual));
		}
	}
}
