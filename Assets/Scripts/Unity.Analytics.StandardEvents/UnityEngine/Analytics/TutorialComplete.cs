namespace UnityEngine.Analytics
{
	[StandardEventName("tutorial_complete", "Onboarding", "Send this event when the player completes a tutorial.")]
	public struct TutorialComplete
	{
		[OptionalParameter("tutorial_id", "The tutorial name or ID.")]
		public string tutorialId;
	}
}
