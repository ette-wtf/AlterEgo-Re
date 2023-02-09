using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.Analytics
{
	[Serializable]
	internal class TrackableProperty
	{
		[Serializable]
		internal class FieldWithTarget
		{
			[SerializeField]
			private string m_ParamName;

			[SerializeField]
			private Object m_Target;

			[SerializeField]
			private string m_FieldPath;

			[SerializeField]
			private string m_TypeString;

			[SerializeField]
			private bool m_DoStatic;

			[SerializeField]
			private string m_StaticString;

			public string paramName
			{
				get
				{
					return m_ParamName;
				}
				set
				{
					m_ParamName = value;
				}
			}

			public Object target
			{
				get
				{
					return m_Target;
				}
				set
				{
					m_Target = value;
				}
			}

			public string fieldPath
			{
				get
				{
					return m_FieldPath;
				}
				set
				{
					m_FieldPath = value;
				}
			}

			public string typeString
			{
				get
				{
					return m_TypeString;
				}
				set
				{
					m_TypeString = value;
				}
			}

			public bool doStatic
			{
				get
				{
					return m_DoStatic;
				}
				set
				{
					m_DoStatic = value;
				}
			}

			public string staticString
			{
				get
				{
					return m_StaticString;
				}
				set
				{
					m_StaticString = value;
				}
			}

			public object GetValue()
			{
				if (m_DoStatic)
				{
					return m_StaticString;
				}
				object value = m_Target;
				string[] array = m_FieldPath.Split('.');
				foreach (string name in array)
				{
					Type type = value.GetType();
					PropertyInfo property = type.GetProperty(name);
					FieldInfo field = type.GetField(name);
					if ((object)property != null)
					{
						value = property.GetValue(value, null);
						continue;
					}
					if ((object)field != null)
					{
						value = field.GetValue(value);
						continue;
					}
					return null;
				}
				return value;
			}
		}

		public const int kMaxParams = 10;

		[SerializeField]
		private List<FieldWithTarget> m_Fields;

		public List<FieldWithTarget> fields
		{
			get
			{
				return m_Fields;
			}
			set
			{
				m_Fields = value;
			}
		}

		public override int GetHashCode()
		{
			int num = 17;
			foreach (FieldWithTarget field in m_Fields)
			{
				num = num * 23 + field.paramName.GetHashCode();
			}
			return num;
		}
	}
}
