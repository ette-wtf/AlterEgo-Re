namespace UnityEngine.Analytics
{
	[StandardEventName("achievement_step", "Engagement", "Send this event when a requirement or step toward completing a multi-part achievement is complete.")]
	public struct AchievementStep
	{
		[RequiredParameter("step_index", "The order of the step.", null)]
		public int stepIndex;

		[RequiredParameter("achievement_id", "A unique id for this achievement.", null)]
		public string achievementId;
	}
}
