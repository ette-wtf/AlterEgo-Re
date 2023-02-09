namespace UnityEngine.Analytics
{
	[StandardEventName("cutscene_skip", "Application", "Send this event when the player opts to skip a cutscene or cinematic screen.")]
	public struct CutsceneSkip
	{
		[RequiredParameter("scene_name", "The name of the cutscene skipped.", null)]
		public string name;
	}
}
