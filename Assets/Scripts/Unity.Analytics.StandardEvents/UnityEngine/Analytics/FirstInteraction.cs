namespace UnityEngine.Analytics
{
	[StandardEventName("first_interaction", "Onboarding", "Send this event with the first voluntary action the user takes after install.")]
	public struct FirstInteraction
	{
		[OptionalParameter("action_id", "The action ID or name. For example, a unique identifier for the button clicked.")]
		public string actionId;
	}
}
