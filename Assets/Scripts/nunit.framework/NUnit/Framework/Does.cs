using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	public static class Does
	{
		public static ConstraintExpression Not
		{
			get
			{
				return new ConstraintExpression().Not;
			}
		}

		public static FileOrDirectoryExistsConstraint Exist
		{
			get
			{
				return new FileOrDirectoryExistsConstraint();
			}
		}

		public static CollectionContainsConstraint Contain(object expected)
		{
			return new CollectionContainsConstraint(expected);
		}

		public static ContainsConstraint Contain(string expected)
		{
			return new ContainsConstraint(expected);
		}

		public static StartsWithConstraint StartWith(string expected)
		{
			return new StartsWithConstraint(expected);
		}

		public static EndsWithConstraint EndWith(string expected)
		{
			return new EndsWithConstraint(expected);
		}

		public static RegexConstraint Match(string pattern)
		{
			return new RegexConstraint(pattern);
		}
	}
}
