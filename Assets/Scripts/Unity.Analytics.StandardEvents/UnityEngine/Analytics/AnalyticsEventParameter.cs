namespace UnityEngine.Analytics
{
	public class AnalyticsEventParameter : AnalyticsEventAttribute
	{
		public string sendName;

		public string tooltip;

		public string groupId;

		public AnalyticsEventParameter(string sendName, string tooltip, string groupId = null)
		{
			this.sendName = sendName;
			this.tooltip = tooltip;
			this.groupId = groupId;
		}
	}
}
