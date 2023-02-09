namespace App
{
	public static class BuildInfo
	{
		public static readonly string[] LANG_TYPE = new string[1] { "日本語" };

		public static readonly string LANG_DEFAULT = "日本語";

		public static readonly string APP_CODENAME = "EGO";

		public static readonly string APP_NAME = "ALTER EGO";

		public static readonly string APP_VERSION_MAIN = "2.4";

		public static readonly string APP_VERSION_SUB = "2";

		public static readonly int APP_VERSION_CODE = 26;

		public static readonly string APP_BUNDLE_ID_BASE = "com.caracolu.alterego";

		public static bool IsRelease = true;

		public static readonly string APP_VERSION = APP_VERSION_MAIN + "." + APP_VERSION_SUB;

		public static readonly string APP_BUNDLE_ID = APP_BUNDLE_ID_BASE;
	}
}
