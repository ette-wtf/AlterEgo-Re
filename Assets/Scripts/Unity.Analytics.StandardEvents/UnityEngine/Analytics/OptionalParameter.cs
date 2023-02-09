using System;

namespace UnityEngine.Analytics
{
	[AttributeUsage(AttributeTargets.Field)]
	public class OptionalParameter : AnalyticsEventParameter
	{
		public OptionalParameter(string sendName, string tooltip)
			: base(sendName, tooltip)
		{
		}
	}
}
