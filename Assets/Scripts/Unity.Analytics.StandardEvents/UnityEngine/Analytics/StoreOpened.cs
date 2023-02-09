namespace UnityEngine.Analytics
{
	[StandardEventName("store_opened", "Monetization", "Send this event when the player opens a store in game.")]
	public struct StoreOpened
	{
		[RequiredParameter("type", "Set to StoreType.Premium if purchases use real-world money; otherwise, StoreType.Soft.", null)]
		public StoreType storeType;
	}
}
