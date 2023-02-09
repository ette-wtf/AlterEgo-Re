using System.Runtime.InteropServices;

namespace UnityEngine.Analytics
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[StandardEventName("chat_message_sent", "Engagement", "Send this event when the player sends a chat message in game.")]
	public struct ChatMessageSent
	{
	}
}
