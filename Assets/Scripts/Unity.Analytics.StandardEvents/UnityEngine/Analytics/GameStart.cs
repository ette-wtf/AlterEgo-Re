using System.Runtime.InteropServices;

namespace UnityEngine.Analytics
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[StandardEventName("game_start", "Progression", "Send this event when gameplay starts. Usually used only in games with an identifiable conclusion.")]
	public struct GameStart
	{
	}
}
