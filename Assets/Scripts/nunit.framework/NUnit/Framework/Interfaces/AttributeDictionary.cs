using System.Collections.Generic;

namespace NUnit.Framework.Interfaces
{
	public class AttributeDictionary : Dictionary<string, string>
	{
		public new string this[string key]
		{
			get
			{
				string value;
				if (TryGetValue(key, out value))
				{
					return value;
				}
				return null;
			}
		}
	}
}
