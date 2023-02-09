namespace UnityEngine.UDP.Analytics
{
	[HideInInspector]
	internal class AnalyticsService
	{
		public enum SessionState
		{
			kSessionStopped = 0,
			kSessionStarted = 1,
			kSessionPaused = 2,
			kSessionResumed = 3
		}

		private const uint kResumeTimeoutInSeconds = 1800u;

		private const string kPlayerSessionIdKey = "udp.player_sessionId";

		private const string kPlayerSessionElapsedTime = "udp.player_session_elapsed_time";

		private const string kPlayerSessionBackgroundTime = "udp.player_session_background_time";

		private static SessionState m_PlayerSessionState;

		private static string m_PlayerSessionId;

		private static ulong m_PlayerSessionElapsedTime;

		private static ulong m_PlayerSessionForegroundTime;

		private static ulong m_PlayerSessionBackgroundTime;

		public static void Initialize()
		{
			m_PlayerSessionState = SessionState.kSessionStopped;
			m_PlayerSessionId = "";
			m_PlayerSessionElapsedTime = 0uL;
			m_PlayerSessionForegroundTime = 0uL;
			m_PlayerSessionBackgroundTime = 0uL;
		}

		public static void OnPlayerQuit()
		{
			onPlayerStateChanged(SessionState.kSessionStopped);
		}

		public static void OnExitPlayMode()
		{
			onPlayerStateChanged(SessionState.kSessionStopped);
		}

		public static void OnPlayerPaused(bool paused)
		{
			onPlayerStateChanged(paused ? SessionState.kSessionPaused : SessionState.kSessionResumed);
		}

		public static void OnDidReloadMonoDomain()
		{
			onPlayerStateChanged(SessionState.kSessionStopped);
		}

		public static void OnAppAwake()
		{
			onPlayerStateChanged(SessionState.kSessionStarted);
		}

		private static void onPlayerStateChanged(SessionState sessionState)
		{
			if (m_PlayerSessionState == sessionState || (m_PlayerSessionState == SessionState.kSessionStopped && sessionState != SessionState.kSessionStarted))
			{
				return;
			}
			ulong currentMillisecondsInUTC = PlatformWrapper.GetCurrentMillisecondsInUTC();
			m_PlayerSessionState = sessionState;
			switch (sessionState)
			{
			case SessionState.kSessionStarted:
			case SessionState.kSessionResumed:
			{
				if (sessionState == SessionState.kSessionStarted)
				{
					m_PlayerSessionId = PlatformWrapper.GetPlayerPrefsString("udp.player_sessionId");
					m_PlayerSessionElapsedTime = PlatformWrapper.GetPlayerPrefsUInt64("udp.player_session_elapsed_time");
					m_PlayerSessionBackgroundTime = PlatformWrapper.GetPlayerPrefsUInt64("udp.player_session_background_time");
				}
				ulong num2 = currentMillisecondsInUTC - m_PlayerSessionBackgroundTime;
				m_PlayerSessionForegroundTime = currentMillisecondsInUTC;
				if (m_PlayerSessionId == "" || m_PlayerSessionElapsedTime == 0L || num2 > 1800000)
				{
					m_PlayerSessionElapsedTime = 0uL;
					m_PlayerSessionId = PlatformWrapper.GenerateRandomId();
					PlatformWrapper.SetPlayerPrefsString("udp.player_sessionId", m_PlayerSessionId);
					PlatformWrapper.SetPlayerPrefsUInt64("udp.player_session_elapsed_time", m_PlayerSessionElapsedTime);
				}
				break;
			}
			case SessionState.kSessionStopped:
				PlatformWrapper.SetPlayerPrefsString("udp.player_sessionId", "");
				PlatformWrapper.SetPlayerPrefsUInt64("udp.player_session_elapsed_time", 0uL);
				PlatformWrapper.SetPlayerPrefsUInt64("udp.player_session_background_time", 0uL);
				break;
			default:
			{
				ulong num = 0uL;
				if (m_PlayerSessionForegroundTime != 0L)
				{
					num = currentMillisecondsInUTC - m_PlayerSessionForegroundTime;
				}
				m_PlayerSessionElapsedTime += num;
				m_PlayerSessionBackgroundTime = currentMillisecondsInUTC;
				PlatformWrapper.SetPlayerPrefsUInt64("udp.player_session_elapsed_time", m_PlayerSessionElapsedTime);
				PlatformWrapper.SetPlayerPrefsUInt64("udp.player_session_background_time", m_PlayerSessionBackgroundTime);
				break;
			}
			}
			AnalyticsClient.OnPlayerSessionStateChanged(m_PlayerSessionState, m_PlayerSessionId, m_PlayerSessionElapsedTime);
		}

		public static ulong GetPlayerSessionElapsedTime()
		{
			if (m_PlayerSessionState == SessionState.kSessionStarted || m_PlayerSessionState == SessionState.kSessionResumed)
			{
				ulong currentMillisecondsInUTC = PlatformWrapper.GetCurrentMillisecondsInUTC();
				ulong num = 0uL;
				if (m_PlayerSessionForegroundTime != 0L)
				{
					num = currentMillisecondsInUTC - m_PlayerSessionForegroundTime;
				}
				return m_PlayerSessionElapsedTime + num;
			}
			return m_PlayerSessionElapsedTime;
		}
	}
}
