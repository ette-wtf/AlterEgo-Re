namespace UnityEngine.Analytics
{
	public class EnumCase : AnalyticsEventAttribute
	{
		public enum Styles
		{
			None = 0,
			Snake = 1,
			Lower = 2
		}

		public Styles Style;

		public EnumCase(Styles style)
		{
			Style = style;
		}
	}
}
