using System;

namespace UnityEngine.Analytics
{
	[AttributeUsage(AttributeTargets.Field)]
	public class RequiredParameter : AnalyticsEventParameter
	{
		public RequiredParameter(string sendName, string tooltip, string groupId = null)
			: base(sendName, tooltip, groupId)
		{
		}
	}
}
