using System;
using System.ComponentModel;
using System.IO;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	public static class FileAssert
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object a, object b)
		{
			throw new InvalidOperationException("FileAssert.Equals should not be used for Assertions");
		}

		public new static void ReferenceEquals(object a, object b)
		{
			throw new InvalidOperationException("FileAssert.ReferenceEquals should not be used for Assertions");
		}

		public static void AreEqual(Stream expected, Stream actual, string message, params object[] args)
		{
			Assert.That(actual, Is.EqualTo(expected), message, args);
		}

		public static void AreEqual(Stream expected, Stream actual)
		{
			AreEqual(expected, actual, string.Empty, null);
		}

		public static void AreEqual(FileInfo expected, FileInfo actual, string message, params object[] args)
		{
			using (FileStream expected2 = expected.OpenRead())
			{
				using (FileStream actual2 = actual.OpenRead())
				{
					AreEqual(expected2, actual2, message, args);
				}
			}
		}

		public static void AreEqual(FileInfo expected, FileInfo actual)
		{
			AreEqual(expected, actual, string.Empty, null);
		}

		public static void AreEqual(string expected, string actual, string message, params object[] args)
		{
			using (FileStream expected2 = File.OpenRead(expected))
			{
				using (FileStream actual2 = File.OpenRead(actual))
				{
					AreEqual(expected2, actual2, message, args);
				}
			}
		}

		public static void AreEqual(string expected, string actual)
		{
			AreEqual(expected, actual, string.Empty, null);
		}

		public static void AreNotEqual(Stream expected, Stream actual, string message, params object[] args)
		{
			Assert.That(actual, Is.Not.EqualTo(expected), message, args);
		}

		public static void AreNotEqual(Stream expected, Stream actual)
		{
			AreNotEqual(expected, actual, string.Empty, null);
		}

		public static void AreNotEqual(FileInfo expected, FileInfo actual, string message, params object[] args)
		{
			using (FileStream expected2 = expected.OpenRead())
			{
				using (FileStream actual2 = actual.OpenRead())
				{
					AreNotEqual(expected2, actual2, message, args);
				}
			}
		}

		public static void AreNotEqual(FileInfo expected, FileInfo actual)
		{
			AreNotEqual(expected, actual, string.Empty, null);
		}

		public static void AreNotEqual(string expected, string actual, string message, params object[] args)
		{
			using (FileStream expected2 = File.OpenRead(expected))
			{
				using (FileStream actual2 = File.OpenRead(actual))
				{
					AreNotEqual(expected2, actual2, message, args);
				}
			}
		}

		public static void AreNotEqual(string expected, string actual)
		{
			AreNotEqual(expected, actual, string.Empty, null);
		}

		public static void Exists(FileInfo actual, string message, params object[] args)
		{
			Assert.That(actual, new FileOrDirectoryExistsConstraint().IgnoreDirectories, message, args);
		}

		public static void Exists(FileInfo actual)
		{
			Exists(actual, string.Empty, null);
		}

		public static void Exists(string actual, string message, params object[] args)
		{
			Assert.That(actual, new FileOrDirectoryExistsConstraint().IgnoreDirectories, message, args);
		}

		public static void Exists(string actual)
		{
			Exists(actual, string.Empty, null);
		}

		public static void DoesNotExist(FileInfo actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new FileOrDirectoryExistsConstraint().IgnoreDirectories), message, args);
		}

		public static void DoesNotExist(FileInfo actual)
		{
			DoesNotExist(actual, string.Empty, null);
		}

		public static void DoesNotExist(string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new FileOrDirectoryExistsConstraint().IgnoreDirectories), message, args);
		}

		public static void DoesNotExist(string actual)
		{
			DoesNotExist(actual, string.Empty, null);
		}
	}
}
