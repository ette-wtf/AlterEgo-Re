using System;
using System.ComponentModel;

namespace NUnit.Framework
{
	public class StringAssert
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object a, object b)
		{
			throw new InvalidOperationException("StringAssert.Equals should not be used for Assertions");
		}

		public new static void ReferenceEquals(object a, object b)
		{
			throw new InvalidOperationException("StringAssert.ReferenceEquals should not be used for Assertions");
		}

		public static void Contains(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, Does.Contain(expected), message, args);
		}

		public static void Contains(string expected, string actual)
		{
			Contains(expected, actual, string.Empty, null);
		}

		public static void DoesNotContain(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, Does.Not.Contain(expected), message, args);
		}

		public static void DoesNotContain(string expected, string actual)
		{
			DoesNotContain(expected, actual, string.Empty, null);
		}

		public static void StartsWith(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, Does.StartWith(expected), message, args);
		}

		public static void StartsWith(string expected, string actual)
		{
			StartsWith(expected, actual, string.Empty, null);
		}

		public static void DoesNotStartWith(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, Does.Not.StartWith(expected), message, args);
		}

		public static void DoesNotStartWith(string expected, string actual)
		{
			DoesNotStartWith(expected, actual, string.Empty, null);
		}

		public static void EndsWith(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, Does.EndWith(expected), message, args);
		}

		public static void EndsWith(string expected, string actual)
		{
			EndsWith(expected, actual, string.Empty, null);
		}

		public static void DoesNotEndWith(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, Does.Not.EndWith(expected), message, args);
		}

		public static void DoesNotEndWith(string expected, string actual)
		{
			DoesNotEndWith(expected, actual, string.Empty, null);
		}

		public static void AreEqualIgnoringCase(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, Is.EqualTo(expected).IgnoreCase, message, args);
		}

		public static void AreEqualIgnoringCase(string expected, string actual)
		{
			AreEqualIgnoringCase(expected, actual, string.Empty, null);
		}

		public static void AreNotEqualIgnoringCase(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, Is.Not.EqualTo(expected).IgnoreCase, message, args);
		}

		public static void AreNotEqualIgnoringCase(string expected, string actual)
		{
			AreNotEqualIgnoringCase(expected, actual, string.Empty, null);
		}

		public static void IsMatch(string pattern, string actual, string message, params object[] args)
		{
			Assert.That(actual, Does.Match(pattern), message, args);
		}

		public static void IsMatch(string pattern, string actual)
		{
			IsMatch(pattern, actual, string.Empty, null);
		}

		public static void DoesNotMatch(string pattern, string actual, string message, params object[] args)
		{
			Assert.That(actual, Does.Not.Match(pattern), message, args);
		}

		public static void DoesNotMatch(string pattern, string actual)
		{
			DoesNotMatch(pattern, actual, string.Empty, null);
		}
	}
}
