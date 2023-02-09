using System;

namespace NUnit.Framework.Internal
{
	public class StringUtil
	{
		public static int Compare(string strA, string strB, bool ignoreCase)
		{
			StringComparison comparisonType = (ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
			return string.Compare(strA, strB, comparisonType);
		}

		public static bool StringsEqual(string strA, string strB, bool ignoreCase)
		{
			return Compare(strA, strB, ignoreCase) == 0;
		}
	}
}
