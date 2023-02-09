namespace UnityEngine.Analytics
{
	[StandardEventName("level_up", "Progression", "Send this event when the player rank or level increases.")]
	public struct LevelUp
	{
		[RequiredParameter("new_level_name", "The new rank or level name.", "level")]
		public string name;

		[RequiredParameter("new_level_index", "The new rank or level index.", "level")]
		public int index;
	}
}
