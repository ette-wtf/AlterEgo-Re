using System;

namespace NUnit.Framework.Constraints
{
	public class InstanceOfTypeConstraint : TypeConstraint
	{
		public override string DisplayName
		{
			get
			{
				return "InstanceOf";
			}
		}

		public InstanceOfTypeConstraint(Type type)
			: base(type, "instance of ")
		{
		}

		protected override bool Matches(object actual)
		{
			return actual != null && expectedType.IsInstanceOfType(actual);
		}
	}
}
