using System;
using System.Collections.Generic;

namespace NCMB.Internal
{
	internal class NCMBIncrementOperation : INCMBFieldOperation
	{
		private object amount;

		public NCMBIncrementOperation(object amount)
		{
			this.amount = amount;
		}

		public object Encode()
		{
			return new Dictionary<string, object>
			{
				{ "__op", "Increment" },
				{ "amount", amount }
			};
		}

		public INCMBFieldOperation MergeWithPrevious(INCMBFieldOperation previous)
		{
			if (previous == null)
			{
				return this;
			}
			if (previous is NCMBDeleteOperation)
			{
				return new NCMBSetOperation(amount);
			}
			if (previous is NCMBSetOperation)
			{
				object value = ((NCMBSetOperation)previous).getValue();
				if (value is string || value == null)
				{
					throw new InvalidOperationException("You cannot increment a non-number.");
				}
				return new NCMBSetOperation(NCMBObject._addNumbers(value, amount));
			}
			if (previous is NCMBIncrementOperation)
			{
				return new NCMBIncrementOperation(NCMBObject._addNumbers(((NCMBIncrementOperation)previous).amount, amount));
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}

		public object Apply(object oldValue, NCMBObject obj, string key)
		{
			if (oldValue == null)
			{
				return amount;
			}
			if (oldValue is string || oldValue == null)
			{
				throw new InvalidOperationException("You cannot increment a non-number.");
			}
			return NCMBObject._addNumbers(oldValue, amount);
		}
	}
}
