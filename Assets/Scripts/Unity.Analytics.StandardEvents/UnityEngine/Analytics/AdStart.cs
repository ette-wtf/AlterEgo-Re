namespace UnityEngine.Analytics
{
	[StandardEventName("ad_start", "Monetization", "Send this event when playback of an ad begins.")]
	public struct AdStart
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
