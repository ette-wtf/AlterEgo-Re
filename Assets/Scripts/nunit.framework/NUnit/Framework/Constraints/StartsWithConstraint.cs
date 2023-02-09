namespace NUnit.Framework.Constraints
{
	public class StartsWithConstraint : StringConstraint
	{
		public StartsWithConstraint(string expected)
			: base(expected)
		{
			descriptionText = "String starting with";
		}

		protected override bool Matches(string actual)
		{
			if (caseInsensitive)
			{
				return actual != null && actual.ToLower().StartsWith(expected.ToLower());
			}
			return actual != null && actual.StartsWith(expected);
		}
	}
}
