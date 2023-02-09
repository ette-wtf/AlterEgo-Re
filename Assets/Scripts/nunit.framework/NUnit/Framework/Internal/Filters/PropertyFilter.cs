using System;
using System.Collections;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class PropertyFilter : ValueMatchFilter
	{
		private string _propertyName;

		protected override string ElementName
		{
			get
			{
				return "prop";
			}
		}

		public PropertyFilter(string propertyName, string expectedValue)
			: base(expectedValue)
		{
			_propertyName = propertyName;
		}

		public override bool Match(ITest test)
		{
			IList list = test.Properties[_propertyName];
			if (list != null)
			{
				foreach (string item in list)
				{
					if (Match(item))
					{
						return true;
					}
				}
			}
			return false;
		}

		public override TNode AddToXml(TNode parentNode, bool recursive)
		{
			TNode tNode = base.AddToXml(parentNode, recursive);
			tNode.AddAttribute("name", _propertyName);
			return tNode;
		}
	}
}
