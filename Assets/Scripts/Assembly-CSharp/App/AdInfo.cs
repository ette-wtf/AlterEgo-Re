using System.Collections.Generic;

namespace App
{
	public static class AdInfo
	{
		private static readonly string ADMOB_APP_ID = "ca-app-pub-4024401978273999~8378034079";

		public static readonly IDictionary<string, string[]> AD_ID = new Dictionary<string, string[]>
		{
			{
				"AdMobBanner",
				new string[2] { ADMOB_APP_ID, "ca-app-pub-4024401978273999/4437406177" }
			},
			{
				"AdMobRectangle",
				new string[4] { ADMOB_APP_ID, "ca-app-pub-4024401978273999/2702201417", "900,750", "0,-90" }
			},
			{
				"AdfuriNative",
				new string[1] { "5ae8316a6a77f0634c00001a" }
			},
			{
				"AdfuriReward",
				new string[1] { "5ae832b321eaef8c13000025" }
			}
		};

		public static bool ENABLE
		{
			get
			{
				return !PurchasingItem.GetByLabel("広告削除");
			}
		}
	}
}
