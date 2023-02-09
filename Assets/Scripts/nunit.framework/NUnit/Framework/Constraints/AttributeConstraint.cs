using System;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
	public class AttributeConstraint : PrefixConstraint
	{
		private readonly Type expectedType;

		private Attribute attrFound;

		public AttributeConstraint(Type type, IConstraint baseConstraint)
			: base(baseConstraint)
		{
			expectedType = type;
			base.DescriptionPrefix = "attribute " + expectedType.FullName;
			if (!typeof(Attribute).GetTypeInfo().IsAssignableFrom(expectedType.GetTypeInfo()))
			{
				throw new ArgumentException(string.Format("Type {0} is not an attribute", expectedType), "type");
			}
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			Guard.ArgumentNotNull(actual, "actual");
			Attribute[] customAttributes = AttributeHelper.GetCustomAttributes(actual, expectedType, true);
			if (customAttributes.Length == 0)
			{
				throw new ArgumentException(string.Format("Attribute {0} was not found", expectedType), "actual");
			}
			attrFound = customAttributes[0];
			return base.BaseConstraint.ApplyTo(attrFound);
		}

		protected override string GetStringRepresentation()
		{
			return string.Format("<attribute {0} {1}>", expectedType, base.BaseConstraint);
		}
	}
}
