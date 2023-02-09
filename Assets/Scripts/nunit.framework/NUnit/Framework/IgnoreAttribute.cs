using System;
using System.Globalization;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class IgnoreAttribute : NUnitAttribute, IApplyToTest
	{
		private string _reason;

		private DateTime? _untilDate;

		private string _until;

		public string Reason
		{
			get
			{
				return Reason;
			}
			private set
			{
			}
		}

		public string Until
		{
			get
			{
				return _until;
			}
			set
			{
				_until = value;
				_untilDate = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
			}
		}

		public IgnoreAttribute(string reason)
		{
			_reason = reason;
		}

		public void ApplyToTest(Test test)
		{
			if (test.RunState == RunState.NotRunnable)
			{
				return;
			}
			if (_untilDate.HasValue)
			{
				if (_untilDate.Value > DateTime.Now)
				{
					test.RunState = RunState.Ignored;
					string value = string.Format("Ignoring until {0}. {1}", _untilDate.Value.ToString("u"), _reason);
					test.Properties.Set("_SKIPREASON", value);
				}
				test.Properties.Set("IgnoreUntilDate", _untilDate.Value.ToString("u"));
			}
			else
			{
				test.RunState = RunState.Ignored;
				test.Properties.Set("_SKIPREASON", _reason);
			}
		}
	}
}
