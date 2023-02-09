using System;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
	public class AssignableFromConstraint : TypeConstraint
	{
		public AssignableFromConstraint(Type type)
			: base(type, "assignable from ")
		{
		}

		protected override bool Matches(object actual)
		{
			return actual != null && actual.GetType().GetTypeInfo().IsAssignableFrom(expectedType.GetTypeInfo());
		}
	}
}
