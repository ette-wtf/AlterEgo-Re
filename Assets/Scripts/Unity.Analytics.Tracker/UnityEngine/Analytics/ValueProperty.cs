using System;

namespace UnityEngine.Analytics
{
	[Serializable]
	public class ValueProperty
	{
		public enum PropertyType
		{
			Disabled = 0,
			Static = 1,
			Dynamic = 2
		}

		[SerializeField]
		private bool m_EditingCustomValue;

		[SerializeField]
		private int m_PopupIndex;

		[SerializeField]
		private string m_CustomValue;

		[SerializeField]
		private bool m_FixedType;

		[SerializeField]
		private string m_EnumType;

		[SerializeField]
		private bool m_EnumTypeIsCustomizable = true;

		[SerializeField]
		private bool m_CanDisable;

		[SerializeField]
		private PropertyType m_PropertyType = PropertyType.Static;

		[SerializeField]
		private string m_ValueType;

		[SerializeField]
		private string m_Value = string.Empty;

		[SerializeField]
		private TrackableField m_Target;

		public string valueType
		{
			get
			{
				return m_ValueType;
			}
			set
			{
				m_ValueType = value;
			}
		}

		public string propertyValue
		{
			get
			{
				if (m_PropertyType == PropertyType.Dynamic && m_Target != null)
				{
					object value = m_Target.GetValue();
					return (value != null) ? value.ToString().Trim() : null;
				}
				return (m_Value != null) ? m_Value.Trim() : null;
			}
		}

		public TrackableField target
		{
			get
			{
				return m_Target;
			}
		}

		public bool IsValid()
		{
			switch (m_PropertyType)
			{
			case PropertyType.Static:
				return !string.IsNullOrEmpty(m_Value) || (object)Type.GetType(m_ValueType) != typeof(string);
			case PropertyType.Dynamic:
				return m_Target != null && m_Target.GetValue() != null;
			default:
				return false;
			}
		}
	}
}
