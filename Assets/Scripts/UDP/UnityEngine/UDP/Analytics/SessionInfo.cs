namespace UnityEngine.UDP.Analytics
{
	[HideInInspector]
	internal class SessionInfo
	{
		private string m_AppId;

		private string m_SessionId;

		private string m_ClientId;

		private string m_DeviceId;

		private string m_Platform;

		private string m_TargetStore;

		private string m_SystemInfo;

		private bool m_Vr;

		public string MAppId
		{
			get
			{
				return m_AppId;
			}
			set
			{
				m_AppId = value;
			}
		}

		public string MSessionId
		{
			get
			{
				return m_SessionId;
			}
			set
			{
				m_SessionId = value;
			}
		}

		public string MClientId
		{
			get
			{
				return m_ClientId;
			}
			set
			{
				m_ClientId = value;
			}
		}

		public string MDeviceId
		{
			get
			{
				return m_DeviceId;
			}
			set
			{
				m_DeviceId = value;
			}
		}

		public string MPlatform
		{
			get
			{
				return m_Platform;
			}
			set
			{
				m_Platform = value;
			}
		}

		public string MTargetStore
		{
			get
			{
				return m_TargetStore;
			}
			set
			{
				m_TargetStore = value;
			}
		}

		public string MSystemInfo
		{
			get
			{
				return m_SystemInfo;
			}
			set
			{
				m_SystemInfo = value;
			}
		}

		public bool MVr
		{
			get
			{
				return m_Vr;
			}
			set
			{
				m_Vr = value;
			}
		}
	}
}
