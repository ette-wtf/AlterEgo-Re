using System;
using System.IO;

namespace NUnit.Framework.Constraints
{
	public class EmptyConstraint : Constraint
	{
		private Constraint realConstraint;

		public override string Description
		{
			get
			{
				return (realConstraint == null) ? "<empty>" : realConstraint.Description;
			}
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			if ((object)actual.GetType() == typeof(string))
			{
				realConstraint = new EmptyStringConstraint();
			}
			else
			{
				if (actual == null)
				{
					throw new ArgumentException("The actual value must be a string or a non-null IEnumerable or DirectoryInfo", "actual");
				}
				if (actual is DirectoryInfo)
				{
					realConstraint = new EmptyDirectoryConstraint();
				}
				else
				{
					realConstraint = new EmptyCollectionConstraint();
				}
			}
			return realConstraint.ApplyTo(actual);
		}
	}
}
