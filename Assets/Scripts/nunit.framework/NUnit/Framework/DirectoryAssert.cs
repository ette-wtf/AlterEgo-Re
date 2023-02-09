using System;
using System.ComponentModel;
using System.IO;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	public static class DirectoryAssert
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object a, object b)
		{
			throw new InvalidOperationException("DirectoryAssert.Equals should not be used for Assertions");
		}

		public new static void ReferenceEquals(object a, object b)
		{
			throw new InvalidOperationException("DirectoryAssert.ReferenceEquals should not be used for Assertions");
		}

		public static void AreEqual(DirectoryInfo expected, DirectoryInfo actual, string message, params object[] args)
		{
			Assert.AreEqual(expected, actual, message, args);
		}

		public static void AreEqual(DirectoryInfo expected, DirectoryInfo actual)
		{
			AreEqual(expected, actual, string.Empty, null);
		}

		public static void AreNotEqual(DirectoryInfo expected, DirectoryInfo actual, string message, params object[] args)
		{
			Assert.AreNotEqual(expected, actual, message, args);
		}

		public static void AreNotEqual(DirectoryInfo expected, DirectoryInfo actual)
		{
			AreNotEqual(expected, actual, string.Empty, null);
		}

		public static void Exists(DirectoryInfo actual, string message, params object[] args)
		{
			Assert.That(actual, new FileOrDirectoryExistsConstraint().IgnoreFiles, message, args);
		}

		public static void Exists(DirectoryInfo actual)
		{
			Exists(actual, string.Empty, null);
		}

		public static void Exists(string actual, string message, params object[] args)
		{
			Assert.That(actual, new FileOrDirectoryExistsConstraint().IgnoreFiles, message, args);
		}

		public static void Exists(string actual)
		{
			Exists(actual, string.Empty, null);
		}

		public static void DoesNotExist(DirectoryInfo actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new FileOrDirectoryExistsConstraint().IgnoreFiles), message, args);
		}

		public static void DoesNotExist(DirectoryInfo actual)
		{
			DoesNotExist(actual, string.Empty, null);
		}

		public static void DoesNotExist(string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new FileOrDirectoryExistsConstraint().IgnoreFiles), message, args);
		}

		public static void DoesNotExist(string actual)
		{
			DoesNotExist(actual, string.Empty, null);
		}
	}
}
