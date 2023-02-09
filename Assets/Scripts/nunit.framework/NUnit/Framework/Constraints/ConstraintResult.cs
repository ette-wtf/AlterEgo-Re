namespace NUnit.Framework.Constraints
{
	public class ConstraintResult
	{
		private IConstraint _constraint;

		public object ActualValue { get; private set; }

		public ConstraintStatus Status { get; set; }

		public virtual bool IsSuccess
		{
			get
			{
				return Status == ConstraintStatus.Success;
			}
		}

		public string Name
		{
			get
			{
				return _constraint.DisplayName;
			}
		}

		public string Description
		{
			get
			{
				return _constraint.Description;
			}
		}

		public ConstraintResult(IConstraint constraint, object actualValue)
		{
			_constraint = constraint;
			ActualValue = actualValue;
		}

		public ConstraintResult(IConstraint constraint, object actualValue, ConstraintStatus status)
			: this(constraint, actualValue)
		{
			Status = status;
		}

		public ConstraintResult(IConstraint constraint, object actualValue, bool isSuccess)
			: this(constraint, actualValue)
		{
			Status = (isSuccess ? ConstraintStatus.Success : ConstraintStatus.Failure);
		}

		public virtual void WriteMessageTo(MessageWriter writer)
		{
			writer.DisplayDifferences(this);
		}

		public virtual void WriteActualValueTo(MessageWriter writer)
		{
			writer.WriteActualValue(ActualValue);
		}
	}
}
