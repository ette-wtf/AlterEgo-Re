namespace UnityEngine.Analytics
{
	[StandardEventName("ad_complete", "Monetization", "Send this event when an ad is successfully viewed and not skipped.")]
	public struct AdComplete
	{
		[RequiredParameter("rewarded", "Set to true if a reward is offered for this ad.", null)]
		public bool rewarded;

		[CustomizableEnum(true)]
		[OptionalParameter("network", "The ad or mediation network provider.")]
		public AdvertisingNetwork network;

		[OptionalParameter("placement_id", "An ad placement or configuration ID.")]
		public string placementId;
	}
}
