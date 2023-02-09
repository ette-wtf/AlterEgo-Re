using System;

namespace UnityEngine.Analytics
{
	[Serializable]
	public class AnalyticsEventParam
	{
		public enum RequirementType
		{
			None = 0,
			Required = 1,
			Optional = 2
		}

		[SerializeField]
		private RequirementType m_RequirementType;

		[SerializeField]
		private string m_GroupID;

		[SerializeField]
		private string m_Tooltip = string.Empty;

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private ValueProperty m_Value;

		public RequirementType requirementType
		{
			get
			{
				return m_RequirementType;
			}
		}

		public string groupID
		{
			get
			{
				return m_GroupID;
			}
		}

		public ValueProperty valueProperty
		{
			get
			{
				return m_Value;
			}
		}

		public string name
		{
			get
			{
				return m_Name;
			}
		}

		public object value
		{
			get
			{
				return m_Value.propertyValue;
			}
		}

		public AnalyticsEventParam(string name)
		{
			m_Name = name.Trim();
		}
	}
}
