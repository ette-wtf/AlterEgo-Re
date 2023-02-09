using System;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
	public class AttributeExistsConstraint : Constraint
	{
		private Type expectedType;

		public override string Description
		{
			get
			{
				return "type with attribute " + MsgUtils.FormatValue(expectedType);
			}
		}

		public AttributeExistsConstraint(Type type)
			: base(type)
		{
			expectedType = type;
			if (!typeof(Attribute).GetTypeInfo().IsAssignableFrom(expectedType.GetTypeInfo()))
			{
				throw new ArgumentException(string.Format("Type {0} is not an attribute", expectedType), "type");
			}
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			Guard.ArgumentNotNull(actual, "actual");
			Attribute[] customAttributes = AttributeHelper.GetCustomAttributes(actual, expectedType, true);
			ConstraintResult constraintResult = new ConstraintResult(this, actual);
			constraintResult.Status = ((customAttributes.Length > 0) ? ConstraintStatus.Success : ConstraintStatus.Failure);
			return constraintResult;
		}
	}
}
