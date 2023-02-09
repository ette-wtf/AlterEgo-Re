using System.Collections.Generic;
using UnityEngine.UDP.Common;

namespace UnityEngine.UDP.Analytics.Events
{
	internal class AppStopEvent : AndroidJavaProxy
	{
		private const string EVENT_NAME = "appRuntimeAppStop";

		private readonly Dictionary<string, object> _params;

		public AppStopEvent(SessionInfo sessionInfo)
			: base("com.unity.udp.sdk.internal.analytics.IEvent")
		{
			_params = Common.GetCommonParams(sessionInfo);
		}

		public string GetEventName()
		{
			return "appRuntimeAppStop";
		}

		public string GetParams()
		{
			return MiniJson.JsonEncode(_params);
		}
	}
}
