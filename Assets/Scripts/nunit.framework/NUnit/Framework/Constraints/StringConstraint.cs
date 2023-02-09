using System;

namespace NUnit.Framework.Constraints
{
	public abstract class StringConstraint : Constraint
	{
		protected string expected;

		protected bool caseInsensitive;

		protected string descriptionText;

		public override string Description
		{
			get
			{
				string text = string.Format("{0} {1}", descriptionText, MsgUtils.FormatValue(expected));
				if (caseInsensitive)
				{
					text += ", ignoring case";
				}
				return text;
			}
		}

		public StringConstraint IgnoreCase
		{
			get
			{
				caseInsensitive = true;
				return this;
			}
		}

		protected StringConstraint()
		{
		}

		protected StringConstraint(string expected)
			: base(expected)
		{
			this.expected = expected;
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			string text = actual as string;
			if (actual != null && text == null)
			{
				throw new ArgumentException("Actual value must be a string", "actual");
			}
			return new ConstraintResult(this, actual, Matches(text));
		}

		protected abstract bool Matches(string actual);
	}
}
