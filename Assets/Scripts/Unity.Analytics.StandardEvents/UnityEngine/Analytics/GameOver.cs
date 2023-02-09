namespace UnityEngine.Analytics
{
	[StandardEventName("game_over", "Progression", "Send this event when gameplay ends (in a game with an identifiable conclusion).")]
	public struct GameOver
	{
		[OptionalParameter("level_index", "The order of this level within the game.")]
		public int index;

		[OptionalParameter("level_name", "The level name.")]
		public string name;
	}
}
