namespace UnityEngine.Store
{
	public class LoginForwardCallback : AndroidJavaProxy
	{
		private ILoginListener loginListener;

		public LoginForwardCallback(ILoginListener loginListener)
			: base("com.unity.channel.sdk.LoginCallback")
		{
			this.loginListener = loginListener;
		}

		public void onInitFinished(int resultCode)
		{
			if (loginListener == null)
			{
				return;
			}
			if (resultCode == ResultCode.SDK_INIT_SUCCESS)
			{
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					loginListener.OnInitialized();
				});
			}
			else
			{
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					loginListener.OnInitializeFailed("Init Failed: " + resultCode);
				});
			}
		}

		public void onLoginFinished(int resultCode, AndroidJavaObject jo)
		{
			if (loginListener == null)
			{
				return;
			}
			if (resultCode == ResultCode.SDK_LOGIN_SUCCESS)
			{
				UserInfo userInfo = new UserInfo();
				userInfo.channel = jo.Call<string>("getChannel", new object[0]);
				string text = jo.Call<string>("getUserId", new object[0]);
				userInfo.userId = userInfo.channel + "_" + text;
				userInfo.userLoginToken = jo.Call<string>("getLoginReceipt", new object[0]);
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					loginListener.OnLogin(userInfo);
				});
			}
			else if (resultCode == ResultCode.SDK_LOGIN_CANCEL)
			{
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					loginListener.OnLoginFailed("User Login Cancel");
				});
			}
			else
			{
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					loginListener.OnLoginFailed("User Login Failed: " + resultCode);
				});
			}
		}
	}
}
