using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	public class Contains
	{
		public static CollectionContainsConstraint Item(object expected)
		{
			return new CollectionContainsConstraint(expected);
		}

		public static DictionaryContainsKeyConstraint Key(object expected)
		{
			return new DictionaryContainsKeyConstraint(expected);
		}

		public static DictionaryContainsValueConstraint Value(object expected)
		{
			return new DictionaryContainsValueConstraint(expected);
		}

		public static SubstringConstraint Substring(string expected)
		{
			return new SubstringConstraint(expected);
		}
	}
}
