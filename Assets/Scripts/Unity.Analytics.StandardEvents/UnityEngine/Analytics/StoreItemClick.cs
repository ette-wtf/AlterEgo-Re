namespace UnityEngine.Analytics
{
	[StandardEventName("store_item_click", "Monetization", "Send this event when the player clicks on an item in the store.")]
	public struct StoreItemClick
	{
		[RequiredParameter("type", "Set to StoreType.Premium if purchases use real-world money; otherwise, StoreType.Soft.", null)]
		public StoreType storeType;

		[RequiredParameter("item_id", "A unique identifier for the item.", null)]
		public string itemId;

		[OptionalParameter("item_name", "The item's name.")]
		public string itemName;
	}
}
