using System;

namespace NUnit.Framework.Constraints
{
	public class ExceptionTypeConstraint : ExactTypeConstraint
	{
		private class ExceptionTypeConstraintResult : ConstraintResult
		{
			private readonly object caughtException;

			public ExceptionTypeConstraintResult(ExceptionTypeConstraint constraint, object caughtException, Type type, bool matches)
				: base(constraint, type, matches)
			{
				this.caughtException = caughtException;
			}

			public override void WriteActualValueTo(MessageWriter writer)
			{
				if (base.Status == ConstraintStatus.Failure)
				{
					Exception ex = caughtException as Exception;
					if (ex == null)
					{
						base.WriteActualValueTo(writer);
					}
					else
					{
						writer.WriteActualValue(ex);
					}
				}
			}
		}

		public ExceptionTypeConstraint(Type type)
			: base(type)
		{
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			Exception ex = actual as Exception;
			if (actual != null && ex == null)
			{
				throw new ArgumentException("Actual value must be an Exception", "actual");
			}
			actualType = ((actual == null) ? null : actual.GetType());
			return new ExceptionTypeConstraintResult(this, actual, actualType, Matches(actual));
		}
	}
}
