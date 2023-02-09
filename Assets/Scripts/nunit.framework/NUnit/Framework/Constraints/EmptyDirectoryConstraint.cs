using System;
using System.IO;

namespace NUnit.Framework.Constraints
{
	public class EmptyDirectoryConstraint : Constraint
	{
		private int files = 0;

		private int subdirs = 0;

		public override string Description
		{
			get
			{
				return "an empty directory";
			}
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			DirectoryInfo directoryInfo = actual as DirectoryInfo;
			if (directoryInfo == null)
			{
				throw new ArgumentException("The actual value must be a DirectoryInfo", "actual");
			}
			files = directoryInfo.GetFiles().Length;
			subdirs = directoryInfo.GetDirectories().Length;
			bool isSuccess = files == 0 && subdirs == 0;
			return new ConstraintResult(this, actual, isSuccess);
		}
	}
}
