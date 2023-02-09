namespace UnityEngine.Analytics
{
	[StandardEventName("screen_visit", "Application", "Send this event when the player opens a menu or visits a screen in the game.")]
	public struct ScreenVisit
	{
		[CustomizableEnum(true)]
		[RequiredParameter("screen_name", "The name of the screen or type of screen visited.", null)]
		public ScreenName screenName;
	}
}
