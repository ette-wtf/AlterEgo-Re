using System;
using System.Reflection;

namespace UnityEngine.Analytics
{
	[Serializable]
	public class TrackableField : TrackablePropertyBase
	{
		[SerializeField]
		private string[] m_ValidTypeNames;

		[SerializeField]
		private string m_Type;

		[SerializeField]
		private string m_EnumType;

		public TrackableField(params Type[] validTypes)
		{
			if (validTypes != null && validTypes.Length != 0)
			{
				m_ValidTypeNames = new string[validTypes.Length];
				for (int i = 0; i < validTypes.Length; i++)
				{
					m_ValidTypeNames[i] = validTypes[i].ToString();
				}
			}
		}

		public object GetValue()
		{
			if (m_Target == null || string.IsNullOrEmpty(m_Path))
			{
				return null;
			}
			object obj = m_Target;
			string[] array = m_Path.Split('.');
			foreach (string name in array)
			{
				try
				{
					PropertyInfo property = obj.GetType().GetProperty(name);
					obj = property.GetValue(obj, null);
				}
				catch
				{
					FieldInfo field = obj.GetType().GetField(name);
					obj = field.GetValue(obj);
				}
			}
			return obj;
		}
	}
}
