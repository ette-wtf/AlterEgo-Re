namespace UnityEngine.Analytics
{
	[StandardEventName("level_quit", "Progression", "Send this event when the player opts to quit from a level before completing it.")]
	public struct LevelQuit
	{
		[RequiredParameter("level_name", "The level name. Either level_name or level_index is required.", "level")]
		public string name;

		[RequiredParameter("level_index", "The order of this level within the game. Either level_name or level_index is required.", "level")]
		public int index;
	}
}
