namespace NUnit.Framework.Constraints
{
	public class EndsWithConstraint : StringConstraint
	{
		public EndsWithConstraint(string expected)
			: base(expected)
		{
			descriptionText = "String ending with";
		}

		protected override bool Matches(string actual)
		{
			if (caseInsensitive)
			{
				return actual != null && actual.ToLower().EndsWith(expected.ToLower());
			}
			return actual != null && actual.EndsWith(expected);
		}
	}
}
