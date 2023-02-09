using System;

namespace NUnit.Framework.Constraints
{
	public class ExactTypeConstraint : TypeConstraint
	{
		public override string DisplayName
		{
			get
			{
				return "TypeOf";
			}
		}

		public ExactTypeConstraint(Type type)
			: base(type, string.Empty)
		{
		}

		protected override bool Matches(object actual)
		{
			return actual != null && (object)actual.GetType() == expectedType;
		}
	}
}
