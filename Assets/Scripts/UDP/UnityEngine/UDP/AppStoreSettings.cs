using System;

namespace UnityEngine.UDP
{
	[Serializable]
	public class AppStoreSettings : ScriptableObject
	{
		public string UnityProjectID = "";

		public string UnityClientID = "";

		public string UnityClientKey = "";

		public string UnityClientRSAPublicKey = "";

		public string AppName = "";

		public string AppSlug = "";

		public string AppItemId = "";

		public string Permission = "";

		internal const string resourceFileName = "UDP Settings";

		public const string appStoreSettingsAssetFolder = "Assets/Plugins/UDP/Resources";

		public const string appStoreSettingsAssetPath = "Assets/Plugins/UDP/Resources/UDP Settings.asset";

		public const string appStoreSettingsPropFolder = "Assets/Plugins/Android/assets";

		public const string appStoreSettingsPropPath = "Assets/Plugins/Android/assets/GameSettings.prop";
	}
}
