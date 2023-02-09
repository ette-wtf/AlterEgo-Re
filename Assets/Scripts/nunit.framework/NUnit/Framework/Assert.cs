using System;
using System.Collections;
using System.ComponentModel;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	public class Assert
	{
		public static void True(bool? condition, string message, params object[] args)
		{
			That(condition, Is.True, message, args);
		}

		public static void True(bool condition, string message, params object[] args)
		{
			That(condition, Is.True, message, args);
		}

		public static void True(bool? condition)
		{
			That(condition, Is.True, null, null);
		}

		public static void True(bool condition)
		{
			That(condition, Is.True, null, null);
		}

		public static void IsTrue(bool? condition, string message, params object[] args)
		{
			That(condition, Is.True, message, args);
		}

		public static void IsTrue(bool condition, string message, params object[] args)
		{
			That(condition, Is.True, message, args);
		}

		public static void IsTrue(bool? condition)
		{
			That(condition, Is.True, null, null);
		}

		public static void IsTrue(bool condition)
		{
			That(condition, Is.True, null, null);
		}

		public static void False(bool? condition, string message, params object[] args)
		{
			That(condition, Is.False, message, args);
		}

		public static void False(bool condition, string message, params object[] args)
		{
			That(condition, Is.False, message, args);
		}

		public static void False(bool? condition)
		{
			That(condition, Is.False, null, null);
		}

		public static void False(bool condition)
		{
			That(condition, Is.False, null, null);
		}

		public static void IsFalse(bool? condition, string message, params object[] args)
		{
			That(condition, Is.False, message, args);
		}

		public static void IsFalse(bool condition, string message, params object[] args)
		{
			That(condition, Is.False, message, args);
		}

		public static void IsFalse(bool? condition)
		{
			That(condition, Is.False, null, null);
		}

		public static void IsFalse(bool condition)
		{
			That(condition, Is.False, null, null);
		}

		public static void NotNull(object anObject, string message, params object[] args)
		{
			That(anObject, Is.Not.Null, message, args);
		}

		public static void NotNull(object anObject)
		{
			That(anObject, Is.Not.Null, null, null);
		}

		public static void IsNotNull(object anObject, string message, params object[] args)
		{
			That(anObject, Is.Not.Null, message, args);
		}

		public static void IsNotNull(object anObject)
		{
			That(anObject, Is.Not.Null, null, null);
		}

		public static void Null(object anObject, string message, params object[] args)
		{
			That(anObject, Is.Null, message, args);
		}

		public static void Null(object anObject)
		{
			That(anObject, Is.Null, null, null);
		}

		public static void IsNull(object anObject, string message, params object[] args)
		{
			That(anObject, Is.Null, message, args);
		}

		public static void IsNull(object anObject)
		{
			That(anObject, Is.Null, null, null);
		}

		public static void IsNaN(double aDouble, string message, params object[] args)
		{
			That(aDouble, Is.NaN, message, args);
		}

		public static void IsNaN(double aDouble)
		{
			That(aDouble, Is.NaN, null, null);
		}

		public static void IsNaN(double? aDouble, string message, params object[] args)
		{
			That(aDouble, Is.NaN, message, args);
		}

		public static void IsNaN(double? aDouble)
		{
			That(aDouble, Is.NaN, null, null);
		}

		public static void IsEmpty(string aString, string message, params object[] args)
		{
			That(aString, new EmptyStringConstraint(), message, args);
		}

		public static void IsEmpty(string aString)
		{
			That(aString, new EmptyStringConstraint(), null, null);
		}

		public static void IsEmpty(IEnumerable collection, string message, params object[] args)
		{
			That(collection, new EmptyCollectionConstraint(), message, args);
		}

		public static void IsEmpty(IEnumerable collection)
		{
			That(collection, new EmptyCollectionConstraint(), null, null);
		}

		public static void IsNotEmpty(string aString, string message, params object[] args)
		{
			That(aString, Is.Not.Empty, message, args);
		}

		public static void IsNotEmpty(string aString)
		{
			That(aString, Is.Not.Empty, null, null);
		}

		public static void IsNotEmpty(IEnumerable collection, string message, params object[] args)
		{
			That(collection, Is.Not.Empty, message, args);
		}

		public static void IsNotEmpty(IEnumerable collection)
		{
			That(collection, Is.Not.Empty, null, null);
		}

		public static void Zero(int actual)
		{
			That(actual, Is.Zero);
		}

		public static void Zero(int actual, string message, params object[] args)
		{
			That(actual, Is.Zero, message, args);
		}

		[CLSCompliant(false)]
		public static void Zero(uint actual)
		{
			That(actual, Is.Zero);
		}

		[CLSCompliant(false)]
		public static void Zero(uint actual, string message, params object[] args)
		{
			That(actual, Is.Zero, message, args);
		}

		public static void Zero(long actual)
		{
			That(actual, Is.Zero);
		}

		public static void Zero(long actual, string message, params object[] args)
		{
			That(actual, Is.Zero, message, args);
		}

		[CLSCompliant(false)]
		public static void Zero(ulong actual)
		{
			That(actual, Is.Zero);
		}

		[CLSCompliant(false)]
		public static void Zero(ulong actual, string message, params object[] args)
		{
			That(actual, Is.Zero, message, args);
		}

		public static void Zero(decimal actual)
		{
			That(actual, Is.Zero);
		}

		public static void Zero(decimal actual, string message, params object[] args)
		{
			That(actual, Is.Zero, message, args);
		}

		public static void Zero(double actual)
		{
			That(actual, Is.Zero);
		}

		public static void Zero(double actual, string message, params object[] args)
		{
			That(actual, Is.Zero, message, args);
		}

		public static void Zero(float actual)
		{
			That(actual, Is.Zero);
		}

		public static void Zero(float actual, string message, params object[] args)
		{
			That(actual, Is.Zero, message, args);
		}

		public static void NotZero(int actual)
		{
			That(actual, Is.Not.Zero);
		}

		public static void NotZero(int actual, string message, params object[] args)
		{
			That(actual, Is.Not.Zero, message, args);
		}

		[CLSCompliant(false)]
		public static void NotZero(uint actual)
		{
			That(actual, Is.Not.Zero);
		}

		[CLSCompliant(false)]
		public static void NotZero(uint actual, string message, params object[] args)
		{
			That(actual, Is.Not.Zero, message, args);
		}

		public static void NotZero(long actual)
		{
			That(actual, Is.Not.Zero);
		}

		public static void NotZero(long actual, string message, params object[] args)
		{
			That(actual, Is.Not.Zero, message, args);
		}

		[CLSCompliant(false)]
		public static void NotZero(ulong actual)
		{
			That(actual, Is.Not.Zero);
		}

		[CLSCompliant(false)]
		public static void NotZero(ulong actual, string message, params object[] args)
		{
			That(actual, Is.Not.Zero, message, args);
		}

		public static void NotZero(decimal actual)
		{
			That(actual, Is.Not.Zero);
		}

		public static void NotZero(decimal actual, string message, params object[] args)
		{
			That(actual, Is.Not.Zero, message, args);
		}

		public static void NotZero(double actual)
		{
			That(actual, Is.Not.Zero);
		}

		public static void NotZero(double actual, string message, params object[] args)
		{
			That(actual, Is.Not.Zero, message, args);
		}

		public static void NotZero(float actual)
		{
			That(actual, Is.Not.Zero);
		}

		public static void NotZero(float actual, string message, params object[] args)
		{
			That(actual, Is.Not.Zero, message, args);
		}

		public static void Positive(int actual)
		{
			That(actual, Is.Positive);
		}

		public static void Positive(int actual, string message, params object[] args)
		{
			That(actual, Is.Positive, message, args);
		}

		[CLSCompliant(false)]
		public static void Positive(uint actual)
		{
			That(actual, Is.Positive);
		}

		[CLSCompliant(false)]
		public static void Positive(uint actual, string message, params object[] args)
		{
			That(actual, Is.Positive, message, args);
		}

		public static void Positive(long actual)
		{
			That(actual, Is.Positive);
		}

		public static void Positive(long actual, string message, params object[] args)
		{
			That(actual, Is.Positive, message, args);
		}

		[CLSCompliant(false)]
		public static void Positive(ulong actual)
		{
			That(actual, Is.Positive);
		}

		[CLSCompliant(false)]
		public static void Positive(ulong actual, string message, params object[] args)
		{
			That(actual, Is.Positive, message, args);
		}

		public static void Positive(decimal actual)
		{
			That(actual, Is.Positive);
		}

		public static void Positive(decimal actual, string message, params object[] args)
		{
			That(actual, Is.Positive, message, args);
		}

		public static void Positive(double actual)
		{
			That(actual, Is.Positive);
		}

		public static void Positive(double actual, string message, params object[] args)
		{
			That(actual, Is.Positive, message, args);
		}

		public static void Positive(float actual)
		{
			That(actual, Is.Positive);
		}

		public static void Positive(float actual, string message, params object[] args)
		{
			That(actual, Is.Positive, message, args);
		}

		public static void Negative(int actual)
		{
			That(actual, Is.Negative);
		}

		public static void Negative(int actual, string message, params object[] args)
		{
			That(actual, Is.Negative, message, args);
		}

		[CLSCompliant(false)]
		public static void Negative(uint actual)
		{
			That(actual, Is.Negative);
		}

		[CLSCompliant(false)]
		public static void Negative(uint actual, string message, params object[] args)
		{
			That(actual, Is.Negative, message, args);
		}

		public static void Negative(long actual)
		{
			That(actual, Is.Negative);
		}

		public static void Negative(long actual, string message, params object[] args)
		{
			That(actual, Is.Negative, message, args);
		}

		[CLSCompliant(false)]
		public static void Negative(ulong actual)
		{
			That(actual, Is.Negative);
		}

		[CLSCompliant(false)]
		public static void Negative(ulong actual, string message, params object[] args)
		{
			That(actual, Is.Negative, message, args);
		}

		public static void Negative(decimal actual)
		{
			That(actual, Is.Negative);
		}

		public static void Negative(decimal actual, string message, params object[] args)
		{
			That(actual, Is.Negative, message, args);
		}

		public static void Negative(double actual)
		{
			That(actual, Is.Negative);
		}

		public static void Negative(double actual, string message, params object[] args)
		{
			That(actual, Is.Negative, message, args);
		}

		public static void Negative(float actual)
		{
			That(actual, Is.Negative);
		}

		public static void Negative(float actual, string message, params object[] args)
		{
			That(actual, Is.Negative, message, args);
		}

		public static void IsAssignableFrom(Type expected, object actual, string message, params object[] args)
		{
			That(actual, Is.AssignableFrom(expected), message, args);
		}

		public static void IsAssignableFrom(Type expected, object actual)
		{
			That(actual, Is.AssignableFrom(expected), null, null);
		}

		public static void IsAssignableFrom<TExpected>(object actual, string message, params object[] args)
		{
			That(actual, Is.AssignableFrom(typeof(TExpected)), message, args);
		}

		public static void IsAssignableFrom<TExpected>(object actual)
		{
			That(actual, Is.AssignableFrom(typeof(TExpected)), null, null);
		}

		public static void IsNotAssignableFrom(Type expected, object actual, string message, params object[] args)
		{
			That(actual, Is.Not.AssignableFrom(expected), message, args);
		}

		public static void IsNotAssignableFrom(Type expected, object actual)
		{
			That(actual, Is.Not.AssignableFrom(expected), null, null);
		}

		public static void IsNotAssignableFrom<TExpected>(object actual, string message, params object[] args)
		{
			That(actual, Is.Not.AssignableFrom(typeof(TExpected)), message, args);
		}

		public static void IsNotAssignableFrom<TExpected>(object actual)
		{
			That(actual, Is.Not.AssignableFrom(typeof(TExpected)), null, null);
		}

		public static void IsInstanceOf(Type expected, object actual, string message, params object[] args)
		{
			That(actual, Is.InstanceOf(expected), message, args);
		}

		public static void IsInstanceOf(Type expected, object actual)
		{
			That(actual, Is.InstanceOf(expected), null, null);
		}

		public static void IsInstanceOf<TExpected>(object actual, string message, params object[] args)
		{
			That(actual, Is.InstanceOf(typeof(TExpected)), message, args);
		}

		public static void IsInstanceOf<TExpected>(object actual)
		{
			That(actual, Is.InstanceOf(typeof(TExpected)), null, null);
		}

		public static void IsNotInstanceOf(Type expected, object actual, string message, params object[] args)
		{
			That(actual, Is.Not.InstanceOf(expected), message, args);
		}

		public static void IsNotInstanceOf(Type expected, object actual)
		{
			That(actual, Is.Not.InstanceOf(expected), null, null);
		}

		public static void IsNotInstanceOf<TExpected>(object actual, string message, params object[] args)
		{
			That(actual, Is.Not.InstanceOf(typeof(TExpected)), message, args);
		}

		public static void IsNotInstanceOf<TExpected>(object actual)
		{
			That(actual, Is.Not.InstanceOf(typeof(TExpected)), null, null);
		}

		public static Exception Throws(IResolveConstraint expression, TestDelegate code, string message, params object[] args)
		{
			Exception ex = null;
			try
			{
				code();
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			That(ex, expression, message, args);
			return ex;
		}

		public static Exception Throws(IResolveConstraint expression, TestDelegate code)
		{
			return Throws(expression, code, string.Empty, null);
		}

		public static Exception Throws(Type expectedExceptionType, TestDelegate code, string message, params object[] args)
		{
			return Throws(new ExceptionTypeConstraint(expectedExceptionType), code, message, args);
		}

		public static Exception Throws(Type expectedExceptionType, TestDelegate code)
		{
			return Throws(new ExceptionTypeConstraint(expectedExceptionType), code, string.Empty, null);
		}

		public static TActual Throws<TActual>(TestDelegate code, string message, params object[] args) where TActual : Exception
		{
			return (TActual)Throws(typeof(TActual), code, message, args);
		}

		public static TActual Throws<TActual>(TestDelegate code) where TActual : Exception
		{
			return Throws<TActual>(code, string.Empty, null);
		}

		public static Exception Catch(TestDelegate code, string message, params object[] args)
		{
			return Throws(new InstanceOfTypeConstraint(typeof(Exception)), code, message, args);
		}

		public static Exception Catch(TestDelegate code)
		{
			return Throws(new InstanceOfTypeConstraint(typeof(Exception)), code);
		}

		public static Exception Catch(Type expectedExceptionType, TestDelegate code, string message, params object[] args)
		{
			return Throws(new InstanceOfTypeConstraint(expectedExceptionType), code, message, args);
		}

		public static Exception Catch(Type expectedExceptionType, TestDelegate code)
		{
			return Throws(new InstanceOfTypeConstraint(expectedExceptionType), code);
		}

		public static TActual Catch<TActual>(TestDelegate code, string message, params object[] args) where TActual : Exception
		{
			return (TActual)Throws(new InstanceOfTypeConstraint(typeof(TActual)), code, message, args);
		}

		public static TActual Catch<TActual>(TestDelegate code) where TActual : Exception
		{
			return (TActual)Throws(new InstanceOfTypeConstraint(typeof(TActual)), code);
		}

		public static void DoesNotThrow(TestDelegate code, string message, params object[] args)
		{
			That(code, new ThrowsNothingConstraint(), message, args);
		}

		public static void DoesNotThrow(TestDelegate code)
		{
			DoesNotThrow(code, string.Empty, null);
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

		public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr)
		{
			That(del, expr.Resolve(), null, null);
		}

		public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, string message, params object[] args)
		{
			IConstraint constraint = expr.Resolve();
			IncrementAssertCount();
			ConstraintResult constraintResult = constraint.ApplyTo(del);
			if (!constraintResult.IsSuccess)
			{
				MessageWriter messageWriter = new TextMessageWriter(message, args);
				constraintResult.WriteMessageTo(messageWriter);
				throw new AssertionException(messageWriter.ToString());
			}
		}

		public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, Func<string> getExceptionMessage)
		{
			IConstraint constraint = expr.Resolve();
			IncrementAssertCount();
			ConstraintResult constraintResult = constraint.ApplyTo(del);
			if (!constraintResult.IsSuccess)
			{
				throw new AssertionException(getExceptionMessage());
			}
		}

		public static void That(TestDelegate code, IResolveConstraint constraint)
		{
			That(code, constraint, null, null);
		}

		public static void That(TestDelegate code, IResolveConstraint constraint, string message, params object[] args)
		{
			That((object)code, constraint, message, args);
		}

		public static void That(TestDelegate code, IResolveConstraint constraint, Func<string> getExceptionMessage)
		{
			That((object)code, constraint, getExceptionMessage);
		}

		public static void That<TActual>(TActual actual, IResolveConstraint expression)
		{
			That(actual, expression, null, null);
		}

		public static void That<TActual>(TActual actual, IResolveConstraint expression, string message, params object[] args)
		{
			IConstraint constraint = expression.Resolve();
			IncrementAssertCount();
			ConstraintResult constraintResult = constraint.ApplyTo(actual);
			if (!constraintResult.IsSuccess)
			{
				MessageWriter messageWriter = new TextMessageWriter(message, args);
				constraintResult.WriteMessageTo(messageWriter);
				throw new AssertionException(messageWriter.ToString());
			}
		}

		public static void That<TActual>(TActual actual, IResolveConstraint expression, Func<string> getExceptionMessage)
		{
			IConstraint constraint = expression.Resolve();
			IncrementAssertCount();
			ConstraintResult constraintResult = constraint.ApplyTo(actual);
			if (!constraintResult.IsSuccess)
			{
				throw new AssertionException(getExceptionMessage());
			}
		}

		public static void ByVal(object actual, IResolveConstraint expression)
		{
			That(actual, expression, null, null);
		}

		public static void ByVal(object actual, IResolveConstraint expression, string message, params object[] args)
		{
			That(actual, expression, message, args);
		}

		protected Assert()
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object a, object b)
		{
			throw new InvalidOperationException("Assert.Equals should not be used for Assertions");
		}

		public new static void ReferenceEquals(object a, object b)
		{
			throw new InvalidOperationException("Assert.ReferenceEquals should not be used for Assertions");
		}

		public static void Pass(string message, params object[] args)
		{
			if (message == null)
			{
				message = string.Empty;
			}
			else if (args != null && args.Length > 0)
			{
				message = string.Format(message, args);
			}
			throw new SuccessException(message);
		}

		public static void Pass(string message)
		{
			Pass(message, null);
		}

		public static void Pass()
		{
			Pass(string.Empty, null);
		}

		public static void Fail(string message, params object[] args)
		{
			if (message == null)
			{
				message = string.Empty;
			}
			else if (args != null && args.Length > 0)
			{
				message = string.Format(message, args);
			}
			throw new AssertionException(message);
		}

		public static void Fail(string message)
		{
			Fail(message, null);
		}

		public static void Fail()
		{
			Fail(string.Empty, null);
		}

		public static void Ignore(string message, params object[] args)
		{
			if (message == null)
			{
				message = string.Empty;
			}
			else if (args != null && args.Length > 0)
			{
				message = string.Format(message, args);
			}
			throw new IgnoreException(message);
		}

		public static void Ignore(string message)
		{
			Ignore(message, null);
		}

		public static void Ignore()
		{
			Ignore(string.Empty, null);
		}

		public static void Inconclusive(string message, params object[] args)
		{
			if (message == null)
			{
				message = string.Empty;
			}
			else if (args != null && args.Length > 0)
			{
				message = string.Format(message, args);
			}
			throw new InconclusiveException(message);
		}

		public static void Inconclusive(string message)
		{
			Inconclusive(message, null);
		}

		public static void Inconclusive()
		{
			Inconclusive(string.Empty, null);
		}

		public static void Contains(object expected, ICollection actual, string message, params object[] args)
		{
			That(actual, new CollectionContainsConstraint(expected), message, args);
		}

		public static void Contains(object expected, ICollection actual)
		{
			That(actual, new CollectionContainsConstraint(expected), null, null);
		}

		public static void Greater(int arg1, int arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThan(arg2), message, args);
		}

		public static void Greater(int arg1, int arg2)
		{
			That(arg1, Is.GreaterThan(arg2), null, null);
		}

		[CLSCompliant(false)]
		public static void Greater(uint arg1, uint arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThan(arg2), message, args);
		}

		[CLSCompliant(false)]
		public static void Greater(uint arg1, uint arg2)
		{
			That(arg1, Is.GreaterThan(arg2), null, null);
		}

		public static void Greater(long arg1, long arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThan(arg2), message, args);
		}

		public static void Greater(long arg1, long arg2)
		{
			That(arg1, Is.GreaterThan(arg2), null, null);
		}

		[CLSCompliant(false)]
		public static void Greater(ulong arg1, ulong arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThan(arg2), message, args);
		}

		[CLSCompliant(false)]
		public static void Greater(ulong arg1, ulong arg2)
		{
			That(arg1, Is.GreaterThan(arg2), null, null);
		}

		public static void Greater(decimal arg1, decimal arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThan(arg2), message, args);
		}

		public static void Greater(decimal arg1, decimal arg2)
		{
			That(arg1, Is.GreaterThan(arg2), null, null);
		}

		public static void Greater(double arg1, double arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThan(arg2), message, args);
		}

		public static void Greater(double arg1, double arg2)
		{
			That(arg1, Is.GreaterThan(arg2), null, null);
		}

		public static void Greater(float arg1, float arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThan(arg2), message, args);
		}

		public static void Greater(float arg1, float arg2)
		{
			That(arg1, Is.GreaterThan(arg2), null, null);
		}

		public static void Greater(IComparable arg1, IComparable arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThan(arg2), message, args);
		}

		public static void Greater(IComparable arg1, IComparable arg2)
		{
			That(arg1, Is.GreaterThan(arg2), null, null);
		}

		public static void Less(int arg1, int arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThan(arg2), message, args);
		}

		public static void Less(int arg1, int arg2)
		{
			That(arg1, Is.LessThan(arg2), null, null);
		}

		[CLSCompliant(false)]
		public static void Less(uint arg1, uint arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThan(arg2), message, args);
		}

		[CLSCompliant(false)]
		public static void Less(uint arg1, uint arg2)
		{
			That(arg1, Is.LessThan(arg2), null, null);
		}

		public static void Less(long arg1, long arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThan(arg2), message, args);
		}

		public static void Less(long arg1, long arg2)
		{
			That(arg1, Is.LessThan(arg2), null, null);
		}

		[CLSCompliant(false)]
		public static void Less(ulong arg1, ulong arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThan(arg2), message, args);
		}

		[CLSCompliant(false)]
		public static void Less(ulong arg1, ulong arg2)
		{
			That(arg1, Is.LessThan(arg2), null, null);
		}

		public static void Less(decimal arg1, decimal arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThan(arg2), message, args);
		}

		public static void Less(decimal arg1, decimal arg2)
		{
			That(arg1, Is.LessThan(arg2), null, null);
		}

		public static void Less(double arg1, double arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThan(arg2), message, args);
		}

		public static void Less(double arg1, double arg2)
		{
			That(arg1, Is.LessThan(arg2), null, null);
		}

		public static void Less(float arg1, float arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThan(arg2), message, args);
		}

		public static void Less(float arg1, float arg2)
		{
			That(arg1, Is.LessThan(arg2), null, null);
		}

		public static void Less(IComparable arg1, IComparable arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThan(arg2), message, args);
		}

		public static void Less(IComparable arg1, IComparable arg2)
		{
			That(arg1, Is.LessThan(arg2), null, null);
		}

		public static void GreaterOrEqual(int arg1, int arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), message, args);
		}

		public static void GreaterOrEqual(int arg1, int arg2)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), null, null);
		}

		[CLSCompliant(false)]
		public static void GreaterOrEqual(uint arg1, uint arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), message, args);
		}

		[CLSCompliant(false)]
		public static void GreaterOrEqual(uint arg1, uint arg2)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), null, null);
		}

		public static void GreaterOrEqual(long arg1, long arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), message, args);
		}

		public static void GreaterOrEqual(long arg1, long arg2)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), null, null);
		}

		[CLSCompliant(false)]
		public static void GreaterOrEqual(ulong arg1, ulong arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), message, args);
		}

		[CLSCompliant(false)]
		public static void GreaterOrEqual(ulong arg1, ulong arg2)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), null, null);
		}

		public static void GreaterOrEqual(decimal arg1, decimal arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), message, args);
		}

		public static void GreaterOrEqual(decimal arg1, decimal arg2)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), null, null);
		}

		public static void GreaterOrEqual(double arg1, double arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), message, args);
		}

		public static void GreaterOrEqual(double arg1, double arg2)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), null, null);
		}

		public static void GreaterOrEqual(float arg1, float arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), message, args);
		}

		public static void GreaterOrEqual(float arg1, float arg2)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), null, null);
		}

		public static void GreaterOrEqual(IComparable arg1, IComparable arg2, string message, params object[] args)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), message, args);
		}

		public static void GreaterOrEqual(IComparable arg1, IComparable arg2)
		{
			That(arg1, Is.GreaterThanOrEqualTo(arg2), null, null);
		}

		public static void LessOrEqual(int arg1, int arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), message, args);
		}

		public static void LessOrEqual(int arg1, int arg2)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), null, null);
		}

		[CLSCompliant(false)]
		public static void LessOrEqual(uint arg1, uint arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), message, args);
		}

		[CLSCompliant(false)]
		public static void LessOrEqual(uint arg1, uint arg2)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), null, null);
		}

		public static void LessOrEqual(long arg1, long arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), message, args);
		}

		public static void LessOrEqual(long arg1, long arg2)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), null, null);
		}

		[CLSCompliant(false)]
		public static void LessOrEqual(ulong arg1, ulong arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), message, args);
		}

		[CLSCompliant(false)]
		public static void LessOrEqual(ulong arg1, ulong arg2)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), null, null);
		}

		public static void LessOrEqual(decimal arg1, decimal arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), message, args);
		}

		public static void LessOrEqual(decimal arg1, decimal arg2)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), null, null);
		}

		public static void LessOrEqual(double arg1, double arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), message, args);
		}

		public static void LessOrEqual(double arg1, double arg2)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), null, null);
		}

		public static void LessOrEqual(float arg1, float arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), message, args);
		}

		public static void LessOrEqual(float arg1, float arg2)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), null, null);
		}

		public static void LessOrEqual(IComparable arg1, IComparable arg2, string message, params object[] args)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), message, args);
		}

		public static void LessOrEqual(IComparable arg1, IComparable arg2)
		{
			That(arg1, Is.LessThanOrEqualTo(arg2), null, null);
		}

		public static void AreEqual(double expected, double actual, double delta, string message, params object[] args)
		{
			AssertDoublesAreEqual(expected, actual, delta, message, args);
		}

		public static void AreEqual(double expected, double actual, double delta)
		{
			AssertDoublesAreEqual(expected, actual, delta, null, null);
		}

		public static void AreEqual(double expected, double? actual, double delta, string message, params object[] args)
		{
			AssertDoublesAreEqual(expected, actual.Value, delta, message, args);
		}

		public static void AreEqual(double expected, double? actual, double delta)
		{
			AssertDoublesAreEqual(expected, actual.Value, delta, null, null);
		}

		public static void AreEqual(object expected, object actual, string message, params object[] args)
		{
			That(actual, Is.EqualTo(expected), message, args);
		}

		public static void AreEqual(object expected, object actual)
		{
			That(actual, Is.EqualTo(expected), null, null);
		}

		public static void AreNotEqual(object expected, object actual, string message, params object[] args)
		{
			That(actual, Is.Not.EqualTo(expected), message, args);
		}

		public static void AreNotEqual(object expected, object actual)
		{
			That(actual, Is.Not.EqualTo(expected), null, null);
		}

		public static void AreSame(object expected, object actual, string message, params object[] args)
		{
			That(actual, Is.SameAs(expected), message, args);
		}

		public static void AreSame(object expected, object actual)
		{
			That(actual, Is.SameAs(expected), null, null);
		}

		public static void AreNotSame(object expected, object actual, string message, params object[] args)
		{
			That(actual, Is.Not.SameAs(expected), message, args);
		}

		public static void AreNotSame(object expected, object actual)
		{
			That(actual, Is.Not.SameAs(expected), null, null);
		}

		protected static void AssertDoublesAreEqual(double expected, double actual, double delta, string message, object[] args)
		{
			if (double.IsNaN(expected) || double.IsInfinity(expected))
			{
				That(actual, Is.EqualTo(expected), message, args);
			}
			else
			{
				That(actual, Is.EqualTo(expected).Within(delta), message, args);
			}
		}

		private static void IncrementAssertCount()
		{
			TestExecutionContext.CurrentContext.IncrementAssertCount();
		}
	}
}
