namespace UnityEngine.Analytics
{
	[StandardEventName("post_ad_action", "Monetization", "Send this event with the first action a player takes after an ad is shown, or after an ad is offered but not shown.")]
	public struct PostAdAction
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
