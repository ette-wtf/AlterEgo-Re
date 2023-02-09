using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class PropertyAttribute : NUnitAttribute, IApplyToTest
	{
		private PropertyBag properties = new PropertyBag();

		public IPropertyBag Properties
		{
			get
			{
				return properties;
			}
		}

		public PropertyAttribute(string propertyName, string propertyValue)
		{
			properties.Add(propertyName, propertyValue);
		}

		public PropertyAttribute(string propertyName, int propertyValue)
		{
			properties.Add(propertyName, propertyValue);
		}

		public PropertyAttribute(string propertyName, double propertyValue)
		{
			properties.Add(propertyName, propertyValue);
		}

		protected PropertyAttribute()
		{
		}

		protected PropertyAttribute(object propertyValue)
		{
			string text = GetType().Name;
			if (text.EndsWith("Attribute"))
			{
				text = text.Substring(0, text.Length - 9);
			}
			properties.Add(text, propertyValue);
		}

		public virtual void ApplyToTest(Test test)
		{
			foreach (string key in Properties.Keys)
			{
				foreach (object item in Properties[key])
				{
					test.Properties.Add(key, item);
				}
			}
		}
	}
}
