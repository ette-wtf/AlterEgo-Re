using System;

namespace UnityEngine.Analytics
{
	[AttributeUsage(AttributeTargets.Field)]
	public class CustomizableEnum : AnalyticsEventAttribute
	{
		public bool Customizable;

		public CustomizableEnum(bool customizable)
		{
			Customizable = customizable;
		}
	}
}
