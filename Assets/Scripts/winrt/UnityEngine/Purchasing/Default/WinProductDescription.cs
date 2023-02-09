namespace UnityEngine.Purchasing.Default
{
	public class WinProductDescription
	{
		public string platformSpecificID { get; private set; }

		public string price { get; private set; }

		public string title { get; private set; }

		public string description { get; private set; }

		public string ISOCurrencyCode { get; private set; }

		public decimal priceDecimal { get; private set; }

		public string receipt { get; private set; }

		public string transactionID { get; private set; }

		public bool consumable { get; private set; }

		public WinProductDescription(string id, string price, string title, string description, string isoCode, decimal priceD, string receipt = null, string transactionId = null, bool consumable = false)
		{
			platformSpecificID = id;
			this.price = price;
			this.title = title;
			this.description = description;
			ISOCurrencyCode = isoCode;
			priceDecimal = priceD;
			this.receipt = receipt;
			transactionID = transactionId;
			this.consumable = consumable;
		}
	}
}
