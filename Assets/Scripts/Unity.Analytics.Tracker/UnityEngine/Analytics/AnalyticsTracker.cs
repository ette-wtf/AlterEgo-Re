using System;
using System.Collections.Generic;

namespace UnityEngine.Analytics
{
	[AddComponentMenu("")]
	[Obsolete("Analytics Tracker is deprecated. Use Analytics Event Tracker instead!")]
	public class AnalyticsTracker : MonoBehaviour
	{
		[Serializable]
		internal enum Trigger
		{
			External = 0,
			Awake = 1,
			Start = 2,
			OnEnable = 3,
			OnDisable = 4,
			OnApplicationPause = 5,
			OnDestroy = 6
		}

		[SerializeField]
		private string m_EventName;

		private Dictionary<string, object> m_Dict = new Dictionary<string, object>();

		private int m_PrevDictHash;

		[SerializeField]
		private TrackableProperty m_TrackableProperty = new TrackableProperty();

		[SerializeField]
		internal Trigger m_Trigger;

		public string eventName
		{
			get
			{
				return m_EventName;
			}
			set
			{
				m_EventName = value;
			}
		}

		internal TrackableProperty TP
		{
			get
			{
				return m_TrackableProperty;
			}
			set
			{
				m_TrackableProperty = value;
			}
		}

		private void Awake()
		{
			if (m_Trigger == Trigger.Awake)
			{
				TriggerEvent();
			}
		}

		private void Start()
		{
			if (m_Trigger == Trigger.Start)
			{
				TriggerEvent();
			}
		}

		private void OnEnable()
		{
			if (m_Trigger == Trigger.OnEnable)
			{
				TriggerEvent();
			}
		}

		private void OnDisable()
		{
			if (m_Trigger == Trigger.OnDisable)
			{
				TriggerEvent();
			}
		}

		private void OnApplicationPause()
		{
			if (m_Trigger == Trigger.OnApplicationPause)
			{
				TriggerEvent();
			}
		}

		private void OnDestroy()
		{
			if (m_Trigger == Trigger.OnDestroy)
			{
				TriggerEvent();
			}
		}

		public void TriggerEvent()
		{
			BuildParameters();
			SendEvent();
		}

		private void SendEvent()
		{
			Analytics.CustomEvent(m_EventName, m_Dict);
		}

		private void BuildParameters()
		{
			int hashCode = m_TrackableProperty.GetHashCode();
			if (hashCode != m_PrevDictHash)
			{
				m_Dict.Clear();
			}
			m_PrevDictHash = hashCode;
			int i = 0;
			for (int count = m_TrackableProperty.fields.Count; i < count; i++)
			{
				TrackableProperty.FieldWithTarget fieldWithTarget = m_TrackableProperty.fields[i];
				if (fieldWithTarget.target != null || fieldWithTarget.doStatic)
				{
					string value = fieldWithTarget.GetValue().ToString();
					m_Dict[fieldWithTarget.paramName] = value;
				}
			}
		}
	}
}
