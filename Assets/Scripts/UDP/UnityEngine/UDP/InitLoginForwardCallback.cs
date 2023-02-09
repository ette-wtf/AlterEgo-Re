namespace UnityEngine.UDP
{
	public class InitLoginForwardCallback : AndroidJavaProxy
	{
		private IInitListener _initListener;

		public InitLoginForwardCallback(IInitListener initListener)
			: base("com.unity.udp.sdk.InitCallback")
		{
			_initListener = initListener;
		}

		public void onInitFinished(int resultCode, string message, AndroidJavaObject jo)
		{
			if (_initListener == null)
			{
				Debug.LogError("LoginCallback cannot be null");
			}
			if (resultCode == 0)
			{
				UserInfo userInfo = null;
				if (jo != null)
				{
					userInfo = new UserInfo();
					userInfo.UserId = userInfo.Channel + "_" + jo.Call<string>("getUserId", new object[0]);
					userInfo.UserLoginToken = jo.Call<string>("getLoginReceipt", new object[0]);
					userInfo.Channel = jo.Call<string>("getChannel", new object[0]);
				}
				if (userInfo == null)
				{
					Debug.Log("No UserInfo is returned.");
				}
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					_initListener.OnInitialized(userInfo);
				});
			}
			else
			{
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					_initListener.OnInitializeFailed(message);
				});
			}
		}
	}
}
