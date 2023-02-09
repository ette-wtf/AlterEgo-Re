using System;

namespace UnityEngine.UDP
{
	[Serializable]
	public class PurchaseInfo
	{
		public string ItemType { get; set; }

		public string ProductId { get; set; }

		public string GameOrderId { get; set; }

		public string OrderQueryToken { get; set; }

		public string DeveloperPayload { get; set; }

		public string StorePurchaseJsonString { get; set; }
	}
}
