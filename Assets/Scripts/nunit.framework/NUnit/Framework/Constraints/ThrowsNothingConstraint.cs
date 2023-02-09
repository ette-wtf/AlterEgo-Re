using System;

namespace NUnit.Framework.Constraints
{
	public class ThrowsNothingConstraint : Constraint
	{
		private Exception caughtException;

		public override string Description
		{
			get
			{
				return "No Exception to be thrown";
			}
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			caughtException = ThrowsConstraint.ExceptionInterceptor.Intercept(actual);
			return new ConstraintResult(this, caughtException, caughtException == null);
		}

		public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
		{
			return ApplyTo(new ThrowsConstraint.GenericInvocationDescriptor<TActual>(del));
		}
	}
}
