namespace UnityEngine.Store
{
	public interface ILoginListener
	{
		void OnInitialized();

		void OnInitializeFailed(string message);

		void OnLogin(UserInfo userInfo);

		void OnLoginFailed(string message);
	}
}
