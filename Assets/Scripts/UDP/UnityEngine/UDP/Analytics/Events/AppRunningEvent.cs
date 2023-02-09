using System.Collections.Generic;
using UnityEngine.UDP.Common;

namespace UnityEngine.UDP.Analytics.Events
{
	internal class AppRunningEvent : AndroidJavaProxy
	{
		private const string EVENT_NAME = "appRuntimeAppRunning";

		private Dictionary<string, object> _params;

		public AppRunningEvent(SessionInfo sessionInfo, ulong duration)
			: base("com.unity.udp.sdk.internal.analytics.IEvent")
		{
			_params = Common.GetCommonParams(sessionInfo);
			_params.Add("duration", duration);
		}

		public string GetEventName()
		{
			return "appRuntimeAppRunning";
		}

		public string GetParams()
		{
			return MiniJson.JsonEncode(_params);
		}
	}
}
