using System;
using System.Reflection;

namespace NUnit.Compatibility
{
	public static class AttributeHelper
	{
		public static Attribute[] GetCustomAttributes(object actual, Type attributeType, bool inherit)
		{
			ICustomAttributeProvider customAttributeProvider = actual as ICustomAttributeProvider;
			if (customAttributeProvider == null)
			{
				throw new ArgumentException(string.Format("Actual value {0} does not implement ICustomAttributeProvider.", actual), "actual");
			}
			return (Attribute[])customAttributeProvider.GetCustomAttributes(attributeType, inherit);
		}
	}
}
