namespace NUnit.Framework.Constraints
{
	public abstract class PrefixConstraint : Constraint
	{
		protected IConstraint BaseConstraint { get; set; }

		protected string DescriptionPrefix { get; set; }

		public override string Description
		{
			get
			{
				return string.Format((BaseConstraint is EqualConstraint) ? "{0} equal to {1}" : "{0} {1}", DescriptionPrefix, BaseConstraint.Description);
			}
		}

		protected PrefixConstraint(IResolveConstraint baseConstraint)
			: base(baseConstraint)
		{
			Guard.ArgumentNotNull(baseConstraint, "baseConstraint");
			BaseConstraint = baseConstraint.Resolve();
		}
	}
}
