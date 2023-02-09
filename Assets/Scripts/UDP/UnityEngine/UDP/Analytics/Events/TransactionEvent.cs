using System.Collections.Generic;
using UnityEngine.UDP.Common;

namespace UnityEngine.UDP.Analytics.Events
{
	internal class TransactionEvent : AndroidJavaProxy
	{
		private const string EVENT_NAME = "appRuntimeTransaction";

		private readonly Dictionary<string, object> _params;

		public TransactionEvent(string cpOrderId, string productId, string currency, string price, string receipt)
			: base("com.unity.udp.sdk.internal.analytics.IEvent")
		{
			SessionInfo sessionInfo = AnalyticsClient.GetSessionInfo();
			_params = Common.GetCommonParams(sessionInfo);
			_params.Add("cp_order_id", cpOrderId);
			_params.Add("product_id", productId);
			_params.Add("receipt", receipt);
			_params.Add("currency", currency);
			_params.Add("price", price);
		}

		public string GetEventName()
		{
			return "appRuntimeTransaction";
		}

		public string GetParams()
		{
			return MiniJson.JsonEncode(_params);
		}
	}
}
