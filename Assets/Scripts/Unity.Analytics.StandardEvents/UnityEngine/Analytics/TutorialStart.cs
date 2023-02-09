namespace UnityEngine.Analytics
{
	[StandardEventName("tutorial_start", "Onboarding", "Send this event when the player starts a tutorial.")]
	public struct TutorialStart
	{
		[OptionalParameter("tutorial_id", "The tutorial name or ID.")]
		public string tutorialId;
	}
}
