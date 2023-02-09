using System;
using System.ComponentModel;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	public class Assume
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object a, object b)
		{
			throw new InvalidOperationException("Assume.Equals should not be used for Assertions");
		}

		public new static void ReferenceEquals(object a, object b)
		{
			throw new InvalidOperationException("Assume.ReferenceEquals should not be used for Assertions");
		}

		public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr)
		{
			That(del, expr.Resolve(), null, null);
		}

		public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, string message, params object[] args)
		{
			IConstraint constraint = expr.Resolve();
			ConstraintResult constraintResult = constraint.ApplyTo(del);
			if (!constraintResult.IsSuccess)
			{
				MessageWriter messageWriter = new TextMessageWriter(message, args);
				constraintResult.WriteMessageTo(messageWriter);
				throw new InconclusiveException(messageWriter.ToString());
			}
		}

		public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, Func<string> getExceptionMessage)
		{
			IConstraint constraint = expr.Resolve();
			ConstraintResult constraintResult = constraint.ApplyTo(del);
			if (!constraintResult.IsSuccess)
			{
				throw new InconclusiveException(getExceptionMessage());
			}
		}

		public static void That(bool condition, string message, params object[] args)
		{
			That(condition, Is.True, message, args);
		}

		public static void That(bool condition)
		{
			That(condition, Is.True, null, null);
		}

		public static void That(bool condition, Func<string> getExceptionMessage)
		{
			That(condition, Is.True, getExceptionMessage);
		}

		public static void That(Func<bool> condition, string message, params object[] args)
		{
			That(condition(), Is.True, message, args);
		}

		public static void That(Func<bool> condition)
		{
			That(condition(), Is.True, null, null);
		}

		public static void That(Func<bool> condition, Func<string> getExceptionMessage)
		{
			That(condition(), Is.True, getExceptionMessage);
		}

		public static void That(TestDelegate code, IResolveConstraint constraint)
		{
			That((object)code, constraint);
		}

		public static void That<TActual>(TActual actual, IResolveConstraint expression)
		{
			That(actual, expression, null, null);
		}

		public static void That<TActual>(TActual actual, IResolveConstraint expression, string message, params object[] args)
		{
			IConstraint constraint = expression.Resolve();
			ConstraintResult constraintResult = constraint.ApplyTo(actual);
			if (!constraintResult.IsSuccess)
			{
				MessageWriter messageWriter = new TextMessageWriter(message, args);
				constraintResult.WriteMessageTo(messageWriter);
				throw new InconclusiveException(messageWriter.ToString());
			}
		}

		public static void That<TActual>(TActual actual, IResolveConstraint expression, Func<string> getExceptionMessage)
		{
			IConstraint constraint = expression.Resolve();
			ConstraintResult constraintResult = constraint.ApplyTo(actual);
			if (!constraintResult.IsSuccess)
			{
				throw new InconclusiveException(getExceptionMessage());
			}
		}
	}
}
