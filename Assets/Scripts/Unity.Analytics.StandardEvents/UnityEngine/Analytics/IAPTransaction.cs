namespace UnityEngine.Analytics
{
	[StandardEventName("iap_transaction", "Monetization", "Send this event when the player spends real-world money to make an In-App Purchase.")]
	public struct IAPTransaction
	{
		[RequiredParameter("transaction_context", "In what context (store, gift, reward) was the item acquired?", null)]
		public string transactionContext;

		[RequiredParameter("price", "How much did the item cost?", null)]
		public float price;

		[RequiredParameter("item_id", "A name or unique identifier for the acquired item.", null)]
		public string itemId;

		[OptionalParameter("item_type", "The category of the item that was acquired.")]
		public string itemType;

		[OptionalParameter("level", "The name or id of the level where the item was acquired.")]
		public string level;

		[OptionalParameter("transaction_id", "A unique identifier for the specific transaction that occurred. You can use this to group multiple events into a single transaction.")]
		public string transactionId;
	}
}
