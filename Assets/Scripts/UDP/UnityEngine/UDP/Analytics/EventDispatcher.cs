namespace UnityEngine.UDP.Analytics
{
	internal class EventDispatcher
	{
		private const string ANALYTICS_DISPATCHER_SERVICE = "com.unity.udp.sdk.internal.analytics.Dispatcher";

		private static AndroidJavaClass serviceClass;

		private static void init()
		{
			serviceClass = new AndroidJavaClass("com.unity.udp.sdk.internal.analytics.Dispatcher");
		}

		public static void DispatchEvent(object e)
		{
			if (serviceClass == null)
			{
				init();
				return;
			}
			serviceClass.CallStatic("Send", e);
		}
	}
}
