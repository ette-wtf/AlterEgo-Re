using System.Collections.Generic;
using UnityEngine.UDP.Common;

namespace UnityEngine.UDP.Analytics.Events
{
	internal class AppInstallEvent : AndroidJavaProxy
	{
		private const string EVENT_NAME = "appRuntimeAppInstall";

		private readonly Dictionary<string, object> _params;

		public AppInstallEvent(SessionInfo sessionInfo)
			: base("com.unity.udp.sdk.internal.analytics.IEvent")
		{
			_params = Common.GetCommonParams(sessionInfo);
		}

		public string GetEventName()
		{
			return "appRuntimeAppInstall";
		}

		public string GetParams()
		{
			return MiniJson.JsonEncode(_params);
		}
	}
}
