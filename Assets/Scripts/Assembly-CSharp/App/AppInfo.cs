using System;

namespace App
{
	public static class AppInfo
	{
		public const string ANALYTICS_VERSION = "EGO_005";

		public const string RESOURCES_FOLDER = "Assets/AppData/Resources";

		public const string APPSTORE_ID = "1447605099";

		public const string SHARE_TAG = "#ALTEREGO";

		public const string SHARE_URL = "caracolu.com/app/alterego/";

		public const string SHARE_DEFAULT = "自分探しタップゲーム『ALTER EGO』 caracolu.com/app/alterego/ #ALTEREGO";

		public static readonly Type[] DefaultComponent = new Type[5]
		{
			typeof(RemoteSettingsReceiver),
			typeof(AudioManager),
			typeof(AdManager),
			typeof(InAppPurchaser),
			typeof(TimeManager)
		};

		public static readonly Type[] PlayerDataList = new Type[11]
		{
			typeof(PurchasingItem),
			typeof(PlayerStatus),
			typeof(BookLevel),
			typeof(PrizeLevel),
			typeof(PlayerResult),
			typeof(CounselingResult),
			typeof(UserChoice),
			typeof(TimeManager),
			typeof(ReadFukidasiList),
			typeof(Settings),
			typeof(DebugFunctions)
		};
	}
}
