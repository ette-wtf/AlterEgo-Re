using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	public class MobileAdsClient : IMobileAdsClient
	{
		private static MobileAdsClient instance = new MobileAdsClient();

		public static MobileAdsClient Instance
		{
			get
			{
				return instance;
			}
		}

		public void Initialize(string appId)
		{
			AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			new AndroidJavaClass("com.google.android.gms.ads.MobileAds").CallStatic("initialize", @static, appId);
		}

		public void SetApplicationVolume(float volume)
		{
			new AndroidJavaClass("com.google.android.gms.ads.MobileAds").CallStatic("setAppVolume", volume);
		}

		public void SetApplicationMuted(bool muted)
		{
			new AndroidJavaClass("com.google.android.gms.ads.MobileAds").CallStatic("setAppMuted", muted);
		}

		public void SetiOSAppPauseOnBackground(bool pause)
		{
		}
	}
}
