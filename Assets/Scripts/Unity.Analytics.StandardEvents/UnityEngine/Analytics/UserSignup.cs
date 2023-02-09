namespace UnityEngine.Analytics
{
	[StandardEventName("user_signup", "Engagement", "Send this event when the player registers or logs in for the first time.")]
	public struct UserSignup
	{
		[RequiredParameter("authorization_network", "The authorization network or login service provider.", null)]
		[CustomizableEnum(true)]
		public AuthorizationNetwork authorizationNetwork;
	}
}
