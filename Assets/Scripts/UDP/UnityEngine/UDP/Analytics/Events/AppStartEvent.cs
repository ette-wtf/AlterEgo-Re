using System.Collections.Generic;
using UnityEngine.UDP.Common;

namespace UnityEngine.UDP.Analytics.Events
{
	internal class AppStartEvent : AndroidJavaProxy
	{
		private const string EVENT_NAME = "appRuntimeAppStart";

		private readonly Dictionary<string, object> _params;

		public AppStartEvent(SessionInfo sessionInfo)
			: base("com.unity.udp.sdk.internal.analytics.IEvent")
		{
			_params = Common.GetCommonParams(sessionInfo);
		}

		public string GetEventName()
		{
			return "appRuntimeAppStart";
		}

		public string GetParams()
		{
			return MiniJson.JsonEncode(_params);
		}
	}
}
