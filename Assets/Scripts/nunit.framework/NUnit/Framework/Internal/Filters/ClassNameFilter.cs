using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class ClassNameFilter : ValueMatchFilter
	{
		protected override string ElementName
		{
			get
			{
				return "class";
			}
		}

		public ClassNameFilter(string expectedValue)
			: base(expectedValue)
		{
		}

		public override bool Match(ITest test)
		{
			if (!test.IsSuite || test is ParameterizedMethodSuite || test.ClassName == null)
			{
				return false;
			}
			return Match(test.ClassName);
		}
	}
}
