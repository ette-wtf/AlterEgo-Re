using System.Collections;

namespace NCMB
{
	public class NCMBPushPayload
	{
		public string PushId { get; protected set; }

		public string Data { get; protected set; }

		public string Title { get; protected set; }

		public string Message { get; protected set; }

		public string Channel { get; protected set; }

		public bool Dialog { get; protected set; }

		public string RichUrl { get; protected set; }

		public IDictionary UserInfo { get; protected set; }

		internal NCMBPushPayload(string pushId, string data, string title, string message, string channel, string dialog, string richUrl, IDictionary userInfo = null)
		{
			PushId = pushId;
			Data = data;
			Title = title;
			Message = message;
			Channel = channel;
			int dialog2;
			switch (dialog)
			{
			default:
				dialog2 = 0;
				break;
			case "true":
			case "TRUE":
			case "True":
			case "1":
				dialog2 = 1;
				break;
			}
			Dialog = (byte)dialog2 != 0;
			RichUrl = richUrl;
			UserInfo = userInfo;
		}
	}
}
