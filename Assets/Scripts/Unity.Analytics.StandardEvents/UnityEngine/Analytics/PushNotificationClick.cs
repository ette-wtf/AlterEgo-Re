namespace UnityEngine.Analytics
{
	[StandardEventName("push_notification_click", "Engagement", "Send this event when the player responds to a push notification.")]
	public struct PushNotificationClick
	{
		[RequiredParameter("message_id", "The message name or ID.", null)]
		public string message_id;
	}
}
