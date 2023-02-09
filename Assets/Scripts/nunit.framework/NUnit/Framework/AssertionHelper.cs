using System.Collections;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	public class AssertionHelper : ConstraintFactory
	{
		public void Expect(bool condition, string message, params object[] args)
		{
			Assert.That(condition, Is.True, message, args);
		}

		public void Expect(bool condition)
		{
			Assert.That(condition, Is.True, null, null);
		}

		public void Expect<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr)
		{
			Assert.That(del, expr.Resolve(), null, null);
		}

		public void Expect<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, string message, params object[] args)
		{
			Assert.That(del, expr, message, args);
		}

		public void Expect(TestDelegate code, IResolveConstraint constraint)
		{
			Assert.That((object)code, constraint);
		}

		public static void Expect<TActual>(TActual actual, IResolveConstraint expression)
		{
			Assert.That(actual, expression, null, null);
		}

		public static void Expect<TActual>(TActual actual, IResolveConstraint expression, string message, params object[] args)
		{
			Assert.That(actual, expression, message, args);
		}

		public ListMapper Map(ICollection original)
		{
			return new ListMapper(original);
		}
	}
}
