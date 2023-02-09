namespace NUnit.Framework.Constraints
{
	public interface IConstraint : IResolveConstraint
	{
		string DisplayName { get; }

		string Description { get; }

		object[] Arguments { get; }

		ConstraintBuilder Builder { get; set; }

		ConstraintResult ApplyTo(object actual);

		ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del);

		ConstraintResult ApplyTo<TActual>(ref TActual actual);
	}
}
