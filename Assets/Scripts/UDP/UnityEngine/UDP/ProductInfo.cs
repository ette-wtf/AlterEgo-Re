using System;

namespace UnityEngine.UDP
{
	[Serializable]
	public class ProductInfo
	{
		public string ItemType { get; set; }

		public string ProductId { get; set; }

		public bool? Consumable { get; set; }

		public string Price { get; set; }

		public long PriceAmountMicros { get; set; }

		public string Currency { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }
	}
}
