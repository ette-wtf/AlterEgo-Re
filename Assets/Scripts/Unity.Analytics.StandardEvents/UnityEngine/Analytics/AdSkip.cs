namespace UnityEngine.Analytics
{
	[StandardEventName("ad_skip", "Monetization", "Send this event when the player opts to skip a video ad during video playback.")]
	public struct AdSkip
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
