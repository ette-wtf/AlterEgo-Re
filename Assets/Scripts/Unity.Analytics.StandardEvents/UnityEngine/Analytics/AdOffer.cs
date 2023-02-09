namespace UnityEngine.Analytics
{
	[StandardEventName("ad_offer", "Monetization", "Send this event when the player is offered the opportunity to view an ad.")]
	public struct AdOffer
	{
		[RequiredParameter("rewarded", "Set to true if a reward is offered for this ad.", null)]
		public bool rewarded;

		[OptionalParameter("network", "The ad or mediation network provider.")]
		[CustomizableEnum(true)]
		public AdvertisingNetwork network;

		[OptionalParameter("placement_id", "An ad placement or configuration ID.")]
		public string placementId;
	}
}
