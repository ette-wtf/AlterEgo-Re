using System;
using System.Diagnostics;
using System.Threading;

namespace NUnit.Framework.Constraints
{
	public class DelayedConstraint : PrefixConstraint
	{
		private readonly int delayInMilliseconds;

		private readonly int pollingInterval;

		public override string Description
		{
			get
			{
				return string.Format("{0} after {1} millisecond delay", base.BaseConstraint.Description, delayInMilliseconds);
			}
		}

		public DelayedConstraint(IConstraint baseConstraint, int delayInMilliseconds)
			: this(baseConstraint, delayInMilliseconds, 0)
		{
		}

		public DelayedConstraint(IConstraint baseConstraint, int delayInMilliseconds, int pollingInterval)
			: base(baseConstraint)
		{
			if (delayInMilliseconds < 0)
			{
				throw new ArgumentException("Cannot check a condition in the past", "delayInMilliseconds");
			}
			this.delayInMilliseconds = delayInMilliseconds;
			this.pollingInterval = pollingInterval;
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			long timestamp = Stopwatch.GetTimestamp();
			long num = TimestampOffset(timestamp, TimeSpan.FromMilliseconds(delayInMilliseconds));
			if (pollingInterval > 0)
			{
				long num2 = TimestampOffset(timestamp, TimeSpan.FromMilliseconds(pollingInterval));
				while ((timestamp = Stopwatch.GetTimestamp()) < num)
				{
					if (num2 > timestamp)
					{
						Thread.Sleep((int)TimestampDiff((num < num2) ? num : num2, timestamp).TotalMilliseconds);
					}
					num2 = TimestampOffset(timestamp, TimeSpan.FromMilliseconds(pollingInterval));
					ConstraintResult constraintResult = base.BaseConstraint.ApplyTo(actual);
					if (constraintResult.IsSuccess)
					{
						return new ConstraintResult(this, actual, true);
					}
				}
			}
			if ((timestamp = Stopwatch.GetTimestamp()) < num)
			{
				Thread.Sleep((int)TimestampDiff(num, timestamp).TotalMilliseconds);
			}
			return new ConstraintResult(this, actual, base.BaseConstraint.ApplyTo(actual).IsSuccess);
		}

		public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
		{
			long timestamp = Stopwatch.GetTimestamp();
			long num = TimestampOffset(timestamp, TimeSpan.FromMilliseconds(delayInMilliseconds));
			object obj;
			if (pollingInterval > 0)
			{
				long num2 = TimestampOffset(timestamp, TimeSpan.FromMilliseconds(pollingInterval));
				while ((timestamp = Stopwatch.GetTimestamp()) < num)
				{
					if (num2 > timestamp)
					{
						Thread.Sleep((int)TimestampDiff((num < num2) ? num : num2, timestamp).TotalMilliseconds);
					}
					num2 = TimestampOffset(timestamp, TimeSpan.FromMilliseconds(pollingInterval));
					obj = InvokeDelegate(del);
					try
					{
						ConstraintResult constraintResult = base.BaseConstraint.ApplyTo(obj);
						if (constraintResult.IsSuccess)
						{
							return new ConstraintResult(this, obj, true);
						}
					}
					catch (Exception)
					{
					}
				}
			}
			if ((timestamp = Stopwatch.GetTimestamp()) < num)
			{
				Thread.Sleep((int)TimestampDiff(num, timestamp).TotalMilliseconds);
			}
			obj = InvokeDelegate(del);
			return new ConstraintResult(this, obj, base.BaseConstraint.ApplyTo(obj).IsSuccess);
		}

		private static object InvokeDelegate<T>(ActualValueDelegate<T> del)
		{
			return del();
		}

		public override ConstraintResult ApplyTo<TActual>(ref TActual actual)
		{
			long timestamp = Stopwatch.GetTimestamp();
			long num = TimestampOffset(timestamp, TimeSpan.FromMilliseconds(delayInMilliseconds));
			if (pollingInterval > 0)
			{
				long num2 = TimestampOffset(timestamp, TimeSpan.FromMilliseconds(pollingInterval));
				while ((timestamp = Stopwatch.GetTimestamp()) < num)
				{
					if (num2 > timestamp)
					{
						Thread.Sleep((int)TimestampDiff((num < num2) ? num : num2, timestamp).TotalMilliseconds);
					}
					num2 = TimestampOffset(timestamp, TimeSpan.FromMilliseconds(pollingInterval));
					try
					{
						ConstraintResult constraintResult = base.BaseConstraint.ApplyTo(actual);
						if (constraintResult.IsSuccess)
						{
							return new ConstraintResult(this, actual, true);
						}
					}
					catch (Exception)
					{
					}
				}
			}
			if ((timestamp = Stopwatch.GetTimestamp()) < num)
			{
				Thread.Sleep((int)TimestampDiff(num, timestamp).TotalMilliseconds);
			}
			return new ConstraintResult(this, actual, base.BaseConstraint.ApplyTo(actual).IsSuccess);
		}

		protected override string GetStringRepresentation()
		{
			return string.Format("<after {0} {1}>", delayInMilliseconds, base.BaseConstraint);
		}

		private static long TimestampOffset(long timestamp, TimeSpan offset)
		{
			return timestamp + (long)(offset.TotalSeconds * (double)Stopwatch.Frequency);
		}

		private static TimeSpan TimestampDiff(long timestamp1, long timestamp2)
		{
			return TimeSpan.FromSeconds((double)(timestamp1 - timestamp2) / (double)Stopwatch.Frequency);
		}
	}
}
