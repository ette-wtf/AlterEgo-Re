namespace UnityEngine.Analytics
{
	[StandardEventName("tutorial_step", "Onboarding", "Send this event when the player completes a step or stage in a multi-part tutorial.")]
	public struct TutorialStep
	{
		[RequiredParameter("step_index", "The step or stage completed in a multi-part tutorial.", null)]
		public int stepIndex;

		[OptionalParameter("tutorial_id", "The tutorial name or ID.")]
		public string tutorialId;
	}
}
