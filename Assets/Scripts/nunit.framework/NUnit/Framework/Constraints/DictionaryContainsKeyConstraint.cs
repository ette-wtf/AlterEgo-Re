using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class DictionaryContainsKeyConstraint : CollectionContainsConstraint
	{
		public override string DisplayName
		{
			get
			{
				return "ContainsKey";
			}
		}

		public override string Description
		{
			get
			{
				return "dictionary containing key " + MsgUtils.FormatValue(base.Expected);
			}
		}

		public DictionaryContainsKeyConstraint(object expected)
			: base(expected)
		{
		}

		protected override bool Matches(IEnumerable actual)
		{
			IDictionary dictionary = actual as IDictionary;
			if (dictionary == null)
			{
				throw new ArgumentException("The actual value must be an IDictionary", "actual");
			}
			return base.Matches((IEnumerable)dictionary.Keys);
		}
	}
}
