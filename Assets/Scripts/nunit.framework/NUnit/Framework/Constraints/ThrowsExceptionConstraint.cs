using System;

namespace NUnit.Framework.Constraints
{
	public class ThrowsExceptionConstraint : Constraint
	{
		private class ThrowsExceptionConstraintResult : ConstraintResult
		{
			public ThrowsExceptionConstraintResult(ThrowsExceptionConstraint constraint, Exception caughtException)
				: base(constraint, caughtException, caughtException != null)
			{
			}

			public override void WriteActualValueTo(MessageWriter writer)
			{
				if (base.Status == ConstraintStatus.Failure)
				{
					writer.Write("no exception thrown");
				}
				else
				{
					base.WriteActualValueTo(writer);
				}
			}
		}

		public override string Description
		{
			get
			{
				return "an exception to be thrown";
			}
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			TestDelegate testDelegate = actual as TestDelegate;
			Exception caughtException = null;
			if (testDelegate != null)
			{
				try
				{
					testDelegate();
				}
				catch (Exception ex)
				{
					caughtException = ex;
				}
				return new ThrowsExceptionConstraintResult(this, caughtException);
			}
			throw new ArgumentException(string.Format("The actual value must be a TestDelegate or AsyncTestDelegate but was {0}", actual.GetType().Name), "actual");
		}

		protected override object GetTestObject<TActual>(ActualValueDelegate<TActual> del)
		{
			return (TestDelegate)delegate
			{
				del();
			};
		}
	}
}
