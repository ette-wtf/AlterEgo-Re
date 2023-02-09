using System;
using System.Collections.Generic;
using App;
using UnityEngine;

public class AnalyticsManager
{
	private static readonly string DeviceInfoKey = "DeviceInfo";

	private static string GetDeviceInfo()
	{
		if (!PlayerPrefs.HasKey(DeviceInfoKey))
		{
			List<string> list = new List<string>();
			string item = Guid.NewGuid().ToString().ToUpper();
			list.Add(item);
			list.Add(SystemInfo.operatingSystem);
			list.Add(SystemInfo.deviceModel);
			list.Add(Screen.height + "x" + Screen.width);
			list.Add(BuildInfo.APP_VERSION);
			PlayerPrefs.SetString(DeviceInfoKey, string.Join("\t", list.ToArray()));
			PlayerPrefs.Save();
		}
		return PlayerPrefs.GetString(DeviceInfoKey);
	}

	public static string GetUserID()
	{
		return GetDeviceInfo().Split('\t')[0].Replace("/editor", "").Replace("/debug", "");
	}

	public static void SendEvent(string[] eventDataList, string ssid = "1DLWkIkaBFBnrV9VoNi09qwvOhocUADC8BWVHfNzyjfA")
	{
		if (!PlayerPrefs.HasKey(DeviceInfoKey))
		{
			GetDeviceInfo();
		}
		string text = "\t";
		text = text + DateTime.Now.ToString() + "\t";
		text = text + GetDeviceInfo() + "\t";
		text = text + BuildInfo.APP_VERSION + "\t";
		text += string.Join("\t", eventDataList);
		text += "\t";
		string[] statusStringArray = StringStatusConverter.GetStatusStringArray(null, false);
		text += statusStringArray[1];
		text = text + SafeAreaAdjuster.GetScreenSize().ToString() + "\t";
		text = text + Screen.safeArea.ToString() + "\t";
		text += SafeAreaAdjuster.SafeAreaHeight;
		GssDataHelper.PostData(ssid, "EGO_005", text);
	}
}
