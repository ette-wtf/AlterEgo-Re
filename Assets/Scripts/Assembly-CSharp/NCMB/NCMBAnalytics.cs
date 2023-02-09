using System;
using System.Collections.Generic;
using MiniJSON;
using NCMB.Internal;

namespace NCMB
{
	[NCMBClassName("analytics")]
	internal class NCMBAnalytics
	{
		internal static void TrackAppOpened(string _pushId)
		{
			if (_pushId == null || NCMBManager._token == null || !NCMBSettings.UseAnalytics)
			{
				return;
			}
			string text = "";
			text = "android";
			string text2 = Json.Serialize(new Dictionary<string, object>
			{
				{ "pushId", _pushId },
				{
					"deviceToken",
					NCMBManager._token
				},
				{ "deviceType", text }
			});
			string url = _getBaseUrl(_pushId);
			ConnectType method = ConnectType.POST;
			string content = text2.ToString();
			new NCMBConnection(url, method, content, NCMBUser._getCurrentSessionToken()).Connect(delegate(int statusCode, string responseData, NCMBException error)
			{
				try
				{
				}
				catch (Exception error2)
				{
					error = new NCMBException(error2);
				}
			});
		}

		internal NCMBAnalytics()
		{
		}

		internal static string _getBaseUrl(string _pushId)
		{
			return NCMBSettings.DomainURL + "/" + NCMBSettings.APIVersion + "/push/" + _pushId + "/openNumber";
		}
	}
}
