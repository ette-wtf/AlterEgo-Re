using System.Collections.Generic;
using UnityEngine.UDP.Common;

namespace UnityEngine.UDP.Analytics.Events
{
	internal class TransactionFailedEvent : AndroidJavaProxy
	{
		private const string EVENT_NAME = "appRuntimeFailedTransaction";

		private readonly Dictionary<string, object> _params;

		public TransactionFailedEvent(string cpOrderId, string productId, string reason)
			: base("com.unity.udp.sdk.internal.analytics.IEvent")
		{
			SessionInfo sessionInfo = AnalyticsClient.GetSessionInfo();
			_params = Common.GetCommonParams(sessionInfo);
			_params.Add("cp_order_id", cpOrderId);
			_params.Add("product_id", productId);
			_params.Add("reason", reason);
		}

		public string GetEventName()
		{
			return "appRuntimeFailedTransaction";
		}

		public string GetParams()
		{
			return MiniJson.JsonEncode(_params);
		}
	}
}
