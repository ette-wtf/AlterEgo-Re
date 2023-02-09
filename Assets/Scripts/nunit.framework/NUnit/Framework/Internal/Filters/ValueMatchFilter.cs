using System;
using System.Text.RegularExpressions;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public abstract class ValueMatchFilter : TestFilter
	{
		public string ExpectedValue { get; private set; }

		public bool IsRegex { get; set; }

		protected abstract string ElementName { get; }

		public ValueMatchFilter(string expectedValue)
		{
			ExpectedValue = expectedValue;
		}

		protected bool Match(string input)
		{
			if (IsRegex)
			{
				return input != null && new Regex(ExpectedValue).IsMatch(input);
			}
			return ExpectedValue == input;
		}

		public override TNode AddToXml(TNode parentNode, bool recursive)
		{
			TNode tNode = parentNode.AddElement(ElementName, ExpectedValue);
			if (IsRegex)
			{
				tNode.AddAttribute("re", "1");
			}
			return tNode;
		}
	}
}
