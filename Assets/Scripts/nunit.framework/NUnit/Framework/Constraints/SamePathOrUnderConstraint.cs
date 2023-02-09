using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
	public class SamePathOrUnderConstraint : PathConstraint
	{
		public override string Description
		{
			get
			{
				return "Path under or matching " + MsgUtils.FormatValue(expected);
			}
		}

		public SamePathOrUnderConstraint(string expected)
			: base(expected)
		{
		}

		protected override bool Matches(string actual)
		{
			if (actual == null)
			{
				return false;
			}
			string text = Canonicalize(expected);
			string text2 = Canonicalize(actual);
			return StringUtil.StringsEqual(text, text2, caseInsensitive) || IsSubPath(text, text2);
		}
	}
}
