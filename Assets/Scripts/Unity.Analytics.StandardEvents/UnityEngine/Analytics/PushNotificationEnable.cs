using System.Runtime.InteropServices;

namespace UnityEngine.Analytics
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[StandardEventName("push_notification_enable", "Engagement", "Send this event when the player enables or grants permission for the game to use push notifications.")]
	public struct PushNotificationEnable
	{
	}
}
