using System;

namespace UnityEngine.Analytics
{
	[AttributeUsage(AttributeTargets.Struct)]
	public class StandardEventName : AnalyticsEventAttribute
	{
		public string sendName;

		public string path;

		public string tooltip;

		public StandardEventName(string sendName, string path, string tooltip)
		{
			this.sendName = sendName;
			this.path = path;
			this.tooltip = tooltip;
		}
	}
}
