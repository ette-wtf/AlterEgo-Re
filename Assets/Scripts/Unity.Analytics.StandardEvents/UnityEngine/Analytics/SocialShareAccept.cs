namespace UnityEngine.Analytics
{
	[StandardEventName("social_share_accept", "Engagement", "Send this event when the player accepts a message, gift, or invitation through social media.")]
	public struct SocialShareAccept
	{
		[CustomizableEnum(true)]
		[RequiredParameter("share_type", "The mode of sharing, or media type used in the social engagement.", null)]
		public ShareType shareType;

		[CustomizableEnum(true)]
		[RequiredParameter("social_network", "The network through which the message is shared.", null)]
		public SocialNetwork socialNetwork;

		[OptionalParameter("sender_id", "A unique identifier for the sender.")]
		public string senderId;

		[OptionalParameter("recipient_id", "A unique identifier for the recipient.")]
		public string recipientId;
	}
}
