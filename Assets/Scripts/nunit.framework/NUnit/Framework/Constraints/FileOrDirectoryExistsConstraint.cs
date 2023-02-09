using System;
using System.IO;

namespace NUnit.Framework.Constraints
{
	public class FileOrDirectoryExistsConstraint : Constraint
	{
		private bool _ignoreDirectories;

		private bool _ignoreFiles;

		public FileOrDirectoryExistsConstraint IgnoreDirectories
		{
			get
			{
				_ignoreDirectories = true;
				return this;
			}
		}

		public FileOrDirectoryExistsConstraint IgnoreFiles
		{
			get
			{
				_ignoreFiles = true;
				return this;
			}
		}

		public override string Description
		{
			get
			{
				if (_ignoreDirectories)
				{
					return "file exists";
				}
				if (_ignoreFiles)
				{
					return "directory exists";
				}
				return "file or directory exists";
			}
		}

		private string ErrorSubstring
		{
			get
			{
				if (_ignoreDirectories)
				{
					return " or FileInfo";
				}
				if (_ignoreFiles)
				{
					return " or DirectoryInfo";
				}
				return ", FileInfo or DirectoryInfo";
			}
		}

		public FileOrDirectoryExistsConstraint()
		{
		}

		public FileOrDirectoryExistsConstraint(bool ignoreDirectories)
		{
			_ignoreDirectories = ignoreDirectories;
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			if (actual == null)
			{
				throw new ArgumentNullException("actual", "The actual value must be a non-null string" + ErrorSubstring);
			}
			if (actual is string)
			{
				return CheckString(actual);
			}
			FileInfo fileInfo = actual as FileInfo;
			if (!_ignoreFiles && fileInfo != null)
			{
				return new ConstraintResult(this, actual, fileInfo.Exists);
			}
			DirectoryInfo directoryInfo = actual as DirectoryInfo;
			if (!_ignoreDirectories && directoryInfo != null)
			{
				return new ConstraintResult(this, actual, directoryInfo.Exists);
			}
			throw new ArgumentException("The actual value must be a string" + ErrorSubstring, "actual");
		}

		private ConstraintResult CheckString<TActual>(TActual actual)
		{
			string text = actual as string;
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentException("The actual value cannot be an empty string", "actual");
			}
			FileInfo fileInfo = new FileInfo(text);
			if (_ignoreDirectories && !_ignoreFiles)
			{
				return new ConstraintResult(this, actual, fileInfo.Exists);
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			if (_ignoreFiles && !_ignoreDirectories)
			{
				return new ConstraintResult(this, actual, directoryInfo.Exists);
			}
			return new ConstraintResult(this, actual, fileInfo.Exists || directoryInfo.Exists);
		}
	}
}
