using System;

namespace UnityEngine.UDP.Analytics
{
	internal class PlatformWrapper
	{
		private const string kAppInstall = "udp.app_install";

		public static ulong GetCurrentMillisecondsInUTC()
		{
			return (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
		}

		public static string GetRuntimePlatformString()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				return "Android";
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.LinuxEditor:
				return "";
			default:
				return "";
			}
		}

		public static string GetSystemInfo()
		{
			return SystemInfo.deviceModel;
		}

		public static string GetPlayerPrefsString(string name)
		{
			return PlayerPrefs.GetString(name, "");
		}

		public static void SetPlayerPrefsString(string name, string value)
		{
			PlayerPrefs.SetString(name, value);
		}

		public static ulong GetPlayerPrefsUInt64(string name)
		{
			return (ulong)PlayerPrefs.GetInt(name, 0);
		}

		public static void SetPlayerPrefsUInt64(string name, ulong value)
		{
			PlayerPrefs.SetInt(name, (int)value);
		}

		public static bool GetAppInstalled()
		{
			return PlayerPrefs.GetInt("udp.app_install", 0) != 0;
		}

		public static void SetAppInstalled(bool v)
		{
			PlayerPrefs.SetInt("udp.app_install", v ? 1 : 0);
		}

		public static string GenerateRandomId()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
