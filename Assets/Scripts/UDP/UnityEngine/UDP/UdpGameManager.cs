using UnityEngine.UDP.Analytics;

namespace UnityEngine.UDP
{
	[HideInInspector]
	internal class UdpGameManager : MonoBehaviour
	{
		public static readonly string OBJECT_NAME = "UnityChannelGameManager";

		private void Awake()
		{
			Debug.Log("udp.gameManager.awake");
			AnalyticsService.OnAppAwake();
		}

		private void Start()
		{
			Debug.Log("udp.gameManager.start");
			Object.DontDestroyOnLoad(base.gameObject);
		}

		private void Update()
		{
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			Debug.Log("udp.gameManager.OnApplicationPause");
			AnalyticsService.OnPlayerPaused(pauseStatus);
		}

		private void OnApplicationQuit()
		{
			Debug.Log("udp.gameManager.OnApplicationQuit");
			AnalyticsService.OnPlayerQuit();
		}
	}
}
