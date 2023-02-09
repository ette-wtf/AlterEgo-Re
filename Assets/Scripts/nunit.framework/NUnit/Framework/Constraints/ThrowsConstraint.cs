using System;

namespace NUnit.Framework.Constraints
{
	public class ThrowsConstraint : PrefixConstraint
	{
		private class ThrowsConstraintResult : ConstraintResult
		{
			private readonly ConstraintResult baseResult;

			public ThrowsConstraintResult(ThrowsConstraint constraint, Exception caughtException, ConstraintResult baseResult)
				: base(constraint, caughtException)
			{
				if (caughtException != null && baseResult.IsSuccess)
				{
					base.Status = ConstraintStatus.Success;
				}
				else
				{
					base.Status = ConstraintStatus.Failure;
				}
				this.baseResult = baseResult;
			}

			public override void WriteActualValueTo(MessageWriter writer)
			{
				if (base.ActualValue == null)
				{
					writer.Write("no exception thrown");
				}
				else
				{
					baseResult.WriteActualValueTo(writer);
				}
			}
		}

		internal class ExceptionInterceptor
		{
			private ExceptionInterceptor()
			{
			}

			internal static Exception Intercept(object invocation)
			{
				IInvocationDescriptor invocationDescriptor = GetInvocationDescriptor(invocation);
				try
				{
					invocationDescriptor.Invoke();
					return null;
				}
				catch (Exception result)
				{
					return result;
				}
			}

			private static IInvocationDescriptor GetInvocationDescriptor(object actual)
			{
				IInvocationDescriptor invocationDescriptor = actual as IInvocationDescriptor;
				if (invocationDescriptor == null)
				{
					TestDelegate testDelegate = actual as TestDelegate;
					if (testDelegate != null)
					{
						invocationDescriptor = new VoidInvocationDescriptor(testDelegate);
					}
				}
				if (invocationDescriptor == null)
				{
					throw new ArgumentException(string.Format("The actual value must be a TestDelegate or AsyncTestDelegate but was {0}", actual.GetType().Name), "actual");
				}
				return invocationDescriptor;
			}
		}

		private interface IInvocationDescriptor
		{
			Delegate Delegate { get; }

			object Invoke();
		}

		internal class GenericInvocationDescriptor<T> : IInvocationDescriptor
		{
			private readonly ActualValueDelegate<T> _del;

			public Delegate Delegate
			{
				get
				{
					return _del;
				}
			}

			public GenericInvocationDescriptor(ActualValueDelegate<T> del)
			{
				_del = del;
			}

			public object Invoke()
			{
				return _del();
			}
		}

		private class VoidInvocationDescriptor : IInvocationDescriptor
		{
			private readonly TestDelegate _del;

			public Delegate Delegate
			{
				get
				{
					return _del;
				}
			}

			public VoidInvocationDescriptor(TestDelegate del)
			{
				_del = del;
			}

			public object Invoke()
			{
				_del();
				return null;
			}
		}

		private Exception caughtException;

		public Exception ActualException
		{
			get
			{
				return caughtException;
			}
		}

		public override string Description
		{
			get
			{
				return base.BaseConstraint.Description;
			}
		}

		public ThrowsConstraint(IConstraint baseConstraint)
			: base(baseConstraint)
		{
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			caughtException = ExceptionInterceptor.Intercept(actual);
			return new ThrowsConstraintResult(this, caughtException, (caughtException != null) ? base.BaseConstraint.ApplyTo(caughtException) : null);
		}

		public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
		{
			return ApplyTo(new GenericInvocationDescriptor<TActual>(del));
		}
	}
}
