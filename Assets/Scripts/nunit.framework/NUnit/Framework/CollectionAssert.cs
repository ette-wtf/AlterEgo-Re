using System;
using System.Collections;
using System.ComponentModel;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	public class CollectionAssert
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object a, object b)
		{
			throw new InvalidOperationException("CollectionAssert.Equals should not be used for Assertions");
		}

		public new static void ReferenceEquals(object a, object b)
		{
			throw new InvalidOperationException("CollectionAssert.ReferenceEquals should not be used for Assertions");
		}

		public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType)
		{
			AllItemsAreInstancesOfType(collection, expectedType, string.Empty, null);
		}

		public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType, string message, params object[] args)
		{
			Assert.That(collection, Is.All.InstanceOf(expectedType), message, args);
		}

		public static void AllItemsAreNotNull(IEnumerable collection)
		{
			AllItemsAreNotNull(collection, string.Empty, null);
		}

		public static void AllItemsAreNotNull(IEnumerable collection, string message, params object[] args)
		{
			Assert.That(collection, Is.All.Not.Null, message, args);
		}

		public static void AllItemsAreUnique(IEnumerable collection)
		{
			AllItemsAreUnique(collection, string.Empty, null);
		}

		public static void AllItemsAreUnique(IEnumerable collection, string message, params object[] args)
		{
			Assert.That(collection, Is.Unique, message, args);
		}

		public static void AreEqual(IEnumerable expected, IEnumerable actual)
		{
			AreEqual(expected, actual, string.Empty, null);
		}

		public static void AreEqual(IEnumerable expected, IEnumerable actual, IComparer comparer)
		{
			AreEqual(expected, actual, comparer, string.Empty, null);
		}

		public static void AreEqual(IEnumerable expected, IEnumerable actual, string message, params object[] args)
		{
			Assert.That(actual, Is.EqualTo(expected), message, args);
		}

		public static void AreEqual(IEnumerable expected, IEnumerable actual, IComparer comparer, string message, params object[] args)
		{
			Assert.That(actual, Is.EqualTo(expected).Using(comparer), message, args);
		}

		public static void AreEquivalent(IEnumerable expected, IEnumerable actual)
		{
			AreEquivalent(expected, actual, string.Empty, null);
		}

		public static void AreEquivalent(IEnumerable expected, IEnumerable actual, string message, params object[] args)
		{
			Assert.That(actual, Is.EquivalentTo(expected), message, args);
		}

		public static void AreNotEqual(IEnumerable expected, IEnumerable actual)
		{
			AreNotEqual(expected, actual, string.Empty, null);
		}

		public static void AreNotEqual(IEnumerable expected, IEnumerable actual, IComparer comparer)
		{
			AreNotEqual(expected, actual, comparer, string.Empty, null);
		}

		public static void AreNotEqual(IEnumerable expected, IEnumerable actual, string message, params object[] args)
		{
			Assert.That(actual, Is.Not.EqualTo(expected), message, args);
		}

		public static void AreNotEqual(IEnumerable expected, IEnumerable actual, IComparer comparer, string message, params object[] args)
		{
			Assert.That(actual, Is.Not.EqualTo(expected).Using(comparer), message, args);
		}

		public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual)
		{
			AreNotEquivalent(expected, actual, string.Empty, null);
		}

		public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual, string message, params object[] args)
		{
			Assert.That(actual, Is.Not.EquivalentTo(expected), message, args);
		}

		public static void Contains(IEnumerable collection, object actual)
		{
			Contains(collection, actual, string.Empty, null);
		}

		public static void Contains(IEnumerable collection, object actual, string message, params object[] args)
		{
			Assert.That(collection, Has.Member(actual), message, args);
		}

		public static void DoesNotContain(IEnumerable collection, object actual)
		{
			DoesNotContain(collection, actual, string.Empty, null);
		}

		public static void DoesNotContain(IEnumerable collection, object actual, string message, params object[] args)
		{
			Assert.That(collection, Has.No.Member(actual), message, args);
		}

		public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset)
		{
			IsNotSubsetOf(subset, superset, string.Empty, null);
		}

		public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset, string message, params object[] args)
		{
			Assert.That(subset, Is.Not.SubsetOf(superset), message, args);
		}

		public static void IsSubsetOf(IEnumerable subset, IEnumerable superset)
		{
			IsSubsetOf(subset, superset, string.Empty, null);
		}

		public static void IsSubsetOf(IEnumerable subset, IEnumerable superset, string message, params object[] args)
		{
			Assert.That(subset, Is.SubsetOf(superset), message, args);
		}

		public static void IsNotSupersetOf(IEnumerable superset, IEnumerable subset)
		{
			IsNotSupersetOf(superset, subset, string.Empty, null);
		}

		public static void IsNotSupersetOf(IEnumerable superset, IEnumerable subset, string message, params object[] args)
		{
			Assert.That(superset, Is.Not.SupersetOf(subset), message, args);
		}

		public static void IsSupersetOf(IEnumerable superset, IEnumerable subset)
		{
			IsSupersetOf(superset, subset, string.Empty, null);
		}

		public static void IsSupersetOf(IEnumerable superset, IEnumerable subset, string message, params object[] args)
		{
			Assert.That(superset, Is.SupersetOf(subset), message, args);
		}

		public static void IsEmpty(IEnumerable collection, string message, params object[] args)
		{
			Assert.That(collection, new EmptyCollectionConstraint(), message, args);
		}

		public static void IsEmpty(IEnumerable collection)
		{
			IsEmpty(collection, string.Empty, null);
		}

		public static void IsNotEmpty(IEnumerable collection, string message, params object[] args)
		{
			Assert.That(collection, new NotConstraint(new EmptyCollectionConstraint()), message, args);
		}

		public static void IsNotEmpty(IEnumerable collection)
		{
			IsNotEmpty(collection, string.Empty, null);
		}

		public static void IsOrdered(IEnumerable collection, string message, params object[] args)
		{
			Assert.That(collection, Is.Ordered, message, args);
		}

		public static void IsOrdered(IEnumerable collection)
		{
			IsOrdered(collection, string.Empty, null);
		}

		public static void IsOrdered(IEnumerable collection, IComparer comparer, string message, params object[] args)
		{
			Assert.That(collection, Is.Ordered.Using(comparer), message, args);
		}

		public static void IsOrdered(IEnumerable collection, IComparer comparer)
		{
			IsOrdered(collection, comparer, string.Empty, null);
		}
	}
}
