using System;

namespace NUnit.Framework.Constraints
{
	public abstract class TypeConstraint : Constraint
	{
		protected Type expectedType;

		protected Type actualType;

		protected TypeConstraint(Type type, string descriptionPrefix)
			: base(type)
		{
			expectedType = type;
			Description = descriptionPrefix + MsgUtils.FormatValue(expectedType);
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			actualType = ((actual == null) ? null : actual.GetType());
			return new ConstraintResult(this, actualType, Matches(actual));
		}

		protected abstract bool Matches(object actual);
	}
}
