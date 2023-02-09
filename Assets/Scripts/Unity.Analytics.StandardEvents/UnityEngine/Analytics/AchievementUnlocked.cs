namespace UnityEngine.Analytics
{
	[StandardEventName("achievement_unlocked", "Engagement", "Send this event when all requirements to unlock an achievement have been met.")]
	public struct AchievementUnlocked
	{
		[RequiredParameter("achievement_id", "A unique id for this achievement.", null)]
		public string achievementId;
	}
}
