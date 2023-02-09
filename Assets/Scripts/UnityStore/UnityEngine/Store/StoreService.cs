namespace UnityEngine.Store
{
	public class StoreService
	{
		private static AndroidJavaClass serviceClass = new AndroidJavaClass("com.unity.channel.sdk.ChannelService");

		public static void Initialize(AppInfo appInfo, ILoginListener listener)
		{
			if (GameObject.Find(MainThreadDispatcher.OBJECT_NAME) == null)
			{
				GameObject gameObject = new GameObject(MainThreadDispatcher.OBJECT_NAME);
				Object.DontDestroyOnLoad(gameObject);
				gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
				gameObject.AddComponent<MainThreadDispatcher>();
			}
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			LoginForwardCallback loginForwardCallback = new LoginForwardCallback(listener);
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.unity.channel.sdk.AppInfo");
			androidJavaObject.Set("appId", appInfo.appId);
			androidJavaObject.Set("appKey", appInfo.appKey);
			androidJavaObject.Set("clientId", appInfo.clientId);
			androidJavaObject.Set("clientSecret", appInfo.clientKey);
			androidJavaObject.Set("debug", appInfo.debug);
			serviceClass.CallStatic("init", @static, androidJavaObject, loginForwardCallback);
		}

		public static void Login(ILoginListener listener)
		{
			LoginForwardCallback loginForwardCallback = new LoginForwardCallback(listener);
			serviceClass.CallStatic("login", loginForwardCallback);
		}
	}
}
