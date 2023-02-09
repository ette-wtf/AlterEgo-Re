using System;

namespace NUnit.Framework.Constraints
{
	[Obsolete("FileExistsConstraint is deprecated, please use FileOrDirectoryExistsConstraint instead.")]
	public class FileExistsConstraint : FileOrDirectoryExistsConstraint
	{
		public override string Description
		{
			get
			{
				return "file exists";
			}
		}

		public FileExistsConstraint()
			: base(true)
		{
		}
	}
}
