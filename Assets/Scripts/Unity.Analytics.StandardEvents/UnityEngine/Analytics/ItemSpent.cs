namespace UnityEngine.Analytics
{
	[StandardEventName("item_spent", "Monetization", "Send this event when the player spends an item.")]
	public struct ItemSpent
	{
		[RequiredParameter("currency_type", "Set to AcquisitionType.Premium if the item was purchased with real money; otherwise, AcqusitionType.Soft.", null)]
		public AcquisitionType currencyType;

		[RequiredParameter("transaction_context", "In what context(store, gift, reward, crafting) was the item spent?", null)]
		public string transactionContext;

		[RequiredParameter("amount", "The unit quantity of the item that was spent", null)]
		public float amount;

		[RequiredParameter("item_id", "A name or unique identifier for the spent item.", null)]
		public string itemId;

		[OptionalParameter("balance", "The balance of the spent item.")]
		public float balance;

		[OptionalParameter("item_type", "The category of the item that was spent.")]
		public string itemType;

		[OptionalParameter("level", "The name or id of the level where the item was spent.")]
		public string level;

		[OptionalParameter("transaction_id", "A unique identifier for the specific transaction that occurred. You can use this to group multiple events into a single transaction.")]
		public string transactionId;
	}
}
