namespace UnityEngine.UDP
{
	public interface IInitListener
	{
		void OnInitialized(UserInfo userInfo);

		void OnInitializeFailed(string message);
	}
}
