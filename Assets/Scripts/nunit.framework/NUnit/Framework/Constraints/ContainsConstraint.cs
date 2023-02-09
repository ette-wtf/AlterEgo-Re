namespace NUnit.Framework.Constraints
{
	public class ContainsConstraint : Constraint
	{
		private readonly object _expected;

		private Constraint _realConstraint;

		private bool _ignoreCase;

		public override string Description
		{
			get
			{
				return (_realConstraint != null) ? _realConstraint.Description : ("containing " + MsgUtils.FormatValue(_expected));
			}
		}

		public ContainsConstraint IgnoreCase
		{
			get
			{
				_ignoreCase = true;
				return this;
			}
		}

		public ContainsConstraint(object expected)
		{
			_expected = expected;
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			if (actual is string)
			{
				StringConstraint stringConstraint = new SubstringConstraint((string)_expected);
				if (_ignoreCase)
				{
					stringConstraint = stringConstraint.IgnoreCase;
				}
				_realConstraint = stringConstraint;
			}
			else
			{
				_realConstraint = new CollectionContainsConstraint(_expected);
			}
			return _realConstraint.ApplyTo(actual);
		}
	}
}
