using System.Collections.Generic;
using UnityEngine.UDP.Common;

namespace UnityEngine.UDP.Analytics.Events
{
	internal class PurchaseAttemptEvent : AndroidJavaProxy
	{
		private const string EVENT_NAME = "appRuntimePurchaseAttempt";

		private readonly Dictionary<string, object> _params;

		public PurchaseAttemptEvent(string productId, string uuid)
			: base("com.unity.udp.sdk.internal.analytics.IEvent")
		{
			SessionInfo sessionInfo = AnalyticsClient.GetSessionInfo();
			_params = Common.GetCommonParams(sessionInfo);
			_params.Add("product_id", productId);
			_params.Add("cp_order_id", uuid);
		}

		public string GetEventName()
		{
			return "appRuntimePurchaseAttempt";
		}

		public string GetParams()
		{
			return MiniJson.JsonEncode(_params);
		}
	}
}
