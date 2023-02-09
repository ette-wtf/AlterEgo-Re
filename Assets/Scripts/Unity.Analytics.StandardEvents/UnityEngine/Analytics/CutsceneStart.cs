namespace UnityEngine.Analytics
{
	[StandardEventName("cutscene_start", "Application", "Send this event when the player begins to watch a cutscene or cinematic screen.")]
	public struct CutsceneStart
	{
		[RequiredParameter("scene_name", "The name of the cutscene being viewed.", null)]
		public string name;
	}
}
