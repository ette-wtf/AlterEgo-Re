using UnityEngine.UDP.Analytics.Events;

namespace UnityEngine.UDP.Analytics
{
	[HideInInspector]
	internal static class AnalyticsClient
	{
		private enum State
		{
			kStateNotReady = 0,
			kStateReady = 1,
			kStatePrepared = 2,
			kStateStarted = 3,
			kStatePaused = 4,
			kStateStopped = 5
		}

		private static SessionInfo m_sessionInfo;

		private static bool m_IsNewSession;

		private static State m_State;

		private static bool m_InStateTransition;

		private static string m_AppId;

		private static string m_ClientId;

		private static string m_TargetStore;

		private static bool m_AppInstalled;

		public static void Initialize(string clientId, string appId, string targetStore)
		{
			m_ClientId = clientId;
			m_AppId = appId;
			m_TargetStore = targetStore;
			m_sessionInfo = new SessionInfo();
			m_AppInstalled = PlatformWrapper.GetAppInstalled();
			RequestStateChange(State.kStateReady);
		}

		public static void OnPlayerSessionStateChanged(AnalyticsService.SessionState sessionState, string sessionId, ulong sessionElapsedTime)
		{
			SetSessionId(sessionId);
			switch (sessionState)
			{
			case AnalyticsService.SessionState.kSessionStopped:
				CloseService();
				return;
			case AnalyticsService.SessionState.kSessionPaused:
				PauseSession();
				return;
			case AnalyticsService.SessionState.kSessionResumed:
				if (!m_IsNewSession)
				{
					ResumeSession();
					return;
				}
				break;
			}
			StartSession();
		}

		private static bool StartSession()
		{
			return RequestStateChange(State.kStateStarted);
		}

		private static bool ResumeSession()
		{
			return RequestStateChange(State.kStateStarted);
		}

		private static bool PauseSession()
		{
			return RequestStateChange(State.kStatePaused);
		}

		private static bool StopSession()
		{
			return RequestStateChange(State.kStateStopped);
		}

		private static bool CloseService()
		{
			if (m_State == State.kStateNotReady)
			{
				return false;
			}
			StopSession();
			return true;
		}

		private static bool RequestStateChange(State state)
		{
			bool result = false;
			if (!m_InStateTransition)
			{
				State nextState = State.kStateNotReady;
				m_InStateTransition = true;
				if (DetermineNextState(state, ref nextState))
				{
					result = ProcessState(nextState);
				}
				m_InStateTransition = false;
			}
			return result;
		}

		private static bool DetermineNextState(State requestedState, ref State nextState)
		{
			nextState = requestedState;
			if (requestedState == m_State)
			{
				return false;
			}
			switch (m_State)
			{
			case State.kStateNotReady:
			case State.kStateStopped:
				if (requestedState != State.kStateReady)
				{
					return false;
				}
				break;
			case State.kStateReady:
				switch (requestedState)
				{
				case State.kStateStarted:
					nextState = State.kStatePrepared;
					break;
				case State.kStatePaused:
					return false;
				}
				break;
			}
			return true;
		}

		private static bool ProcessState(State nextState)
		{
			switch (nextState)
			{
			case State.kStateReady:
				OnEnterStateReady();
				OnEnterStatePrepared();
				break;
			case State.kStatePrepared:
				OnEnterStatePrepared();
				break;
			case State.kStateStarted:
				OnEnterStateStarted();
				break;
			case State.kStatePaused:
				OnEnterStatePaused();
				break;
			case State.kStateStopped:
				OnEnterStateStopped();
				break;
			default:
				return false;
			}
			return true;
		}

		private static void OnEnterStateReady()
		{
			SetState(State.kStateReady);
			m_sessionInfo.MAppId = m_AppId;
			m_sessionInfo.MClientId = m_ClientId;
			m_sessionInfo.MTargetStore = m_TargetStore;
			m_sessionInfo.MPlatform = PlatformWrapper.GetRuntimePlatformString();
			m_sessionInfo.MSystemInfo = PlatformWrapper.GetSystemInfo();
			m_sessionInfo.MDeviceId = SystemInfo.deviceUniqueIdentifier;
			m_sessionInfo.MVr = false;
		}

		private static void OnEnterStatePrepared()
		{
			State state = m_State;
			SetState(State.kStatePrepared);
			int num = 4;
		}

		private static void OnEnterStateStarted()
		{
			SetState(State.kStateStarted);
			if (m_IsNewSession)
			{
				EventDispatcher.DispatchEvent(new AppStartEvent(m_sessionInfo));
				if (!m_AppInstalled)
				{
					EventDispatcher.DispatchEvent(new AppInstallEvent(m_sessionInfo));
					m_AppInstalled = true;
					SavePersistentValue();
				}
			}
			m_IsNewSession = false;
		}

		private static void OnEnterStatePaused()
		{
			OnEnteringStatePausedOrStopped();
			SetState(State.kStatePaused);
		}

		private static void OnEnterStateStopped()
		{
			if (m_State == State.kStateStarted)
			{
				OnEnteringStatePausedOrStopped();
			}
			EventDispatcher.DispatchEvent(new AppStopEvent(m_sessionInfo));
		}

		private static void OnEnteringStatePausedOrStopped()
		{
			SendAppRunningEvent();
			SavePersistentValue();
		}

		private static void SendAppRunningEvent()
		{
			EventDispatcher.DispatchEvent(new AppRunningEvent(m_sessionInfo, GetPlayerSessionElapsedTime()));
		}

		private static void SavePersistentValue()
		{
			PlatformWrapper.SetAppInstalled(m_AppInstalled);
		}

		private static ulong GetPlayerSessionElapsedTime()
		{
			return AnalyticsService.GetPlayerSessionElapsedTime();
		}

		private static void SetSessionId(string sessionId)
		{
			m_IsNewSession = m_sessionInfo.MSessionId != sessionId;
			m_sessionInfo.MSessionId = sessionId;
		}

		private static void SetState(State state)
		{
			m_State = state;
		}

		public static SessionInfo GetSessionInfo()
		{
			return m_sessionInfo;
		}

		public static string GetSessionId()
		{
			if (m_sessionInfo != null)
			{
				return m_sessionInfo.MSessionId;
			}
			return "";
		}
	}
}
