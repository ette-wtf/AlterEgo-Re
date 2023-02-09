using System;
using System.Reflection;

namespace NUnit.Framework.Constraints
{
	public class PropertyExistsConstraint : Constraint
	{
		private readonly string name;

		private Type actualType;

		public override string Description
		{
			get
			{
				return "property " + name;
			}
		}

		public PropertyExistsConstraint(string name)
			: base(name)
		{
			this.name = name;
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			Guard.ArgumentNotNull(actual, "actual");
			actualType = actual as Type;
			if ((object)actualType == null)
			{
				actualType = actual.GetType();
			}
			PropertyInfo property = actualType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return new ConstraintResult(this, actualType, (object)property != null);
		}

		protected override string GetStringRepresentation()
		{
			return string.Format("<propertyexists {0}>", name);
		}
	}
}
