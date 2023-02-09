namespace UnityEngine.Analytics
{
	[StandardEventName("item_acquired", "Monetization", "Send this event when the player acquires an item within the game.")]
	public struct ItemAcquired
	{
		[RequiredParameter("currency_type", "Set to AcquisitionType.Premium if the item was purchased with real money; otherwise, AcqusitionType.Soft.", null)]
		public AcquisitionType currencyType;

		[RequiredParameter("transaction_context", "In what context(store, gift, reward, crafting) was the item acquired?", null)]
		public string transactionContext;

		[RequiredParameter("amount", "The unit quantity of the item that was acquired", null)]
		public float amount;

		[RequiredParameter("item_id", "A name or unique identifier for the acquired item.", null)]
		public string itemId;

		[OptionalParameter("balance", "The balance of the acquired item.")]
		public float balance;

		[OptionalParameter("item_type", "The category of the item that was acquired.")]
		public string itemType;

		[OptionalParameter("level", "The name or id of the level where the item was acquired.")]
		public string level;

		[OptionalParameter("transaction_id", "A unique identifier for the specific transaction that occurred. You can use this to group multiple events into a single transaction.")]
		public string transactionId;
	}
}
