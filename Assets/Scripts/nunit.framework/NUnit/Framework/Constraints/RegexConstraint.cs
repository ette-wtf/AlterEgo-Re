using System.Text.RegularExpressions;

namespace NUnit.Framework.Constraints
{
	public class RegexConstraint : StringConstraint
	{
		public RegexConstraint(string pattern)
			: base(pattern)
		{
			descriptionText = "String matching";
		}

		protected override bool Matches(string actual)
		{
			return actual != null && Regex.IsMatch(actual, expected, caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
		}
	}
}
