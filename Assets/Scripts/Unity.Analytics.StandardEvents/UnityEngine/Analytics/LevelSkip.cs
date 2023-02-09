namespace UnityEngine.Analytics
{
	[StandardEventName("level_skip", "Progression", "Send this event when the player opts to skip past a level.")]
	public struct LevelSkip
	{
		[RequiredParameter("level_name", "The level name. Either level_name or level_index is required.", "level")]
		public string name;

		[RequiredParameter("level_index", "The order of this level within the game. Either level_name or level_index is required.", "level")]
		public int index;
	}
}
