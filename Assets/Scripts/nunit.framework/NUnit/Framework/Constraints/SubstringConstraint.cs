namespace NUnit.Framework.Constraints
{
	public class SubstringConstraint : StringConstraint
	{
		public SubstringConstraint(string expected)
			: base(expected)
		{
			descriptionText = "String containing";
		}

		protected override bool Matches(string actual)
		{
			if (caseInsensitive)
			{
				return actual != null && actual.ToLower().IndexOf(expected.ToLower()) >= 0;
			}
			return actual != null && actual.IndexOf(expected) >= 0;
		}
	}
}
