namespace UnityEngine.Analytics
{
	[StandardEventName("tutorial_skip", "Onboarding", "Send this event when the player opts to skip a tutorial.")]
	public struct TutorialSkip
	{
		[OptionalParameter("tutorial_id", "The tutorial name or ID.")]
		public string tutorialId;
	}
}
