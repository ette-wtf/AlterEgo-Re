using System;
using System.Reflection;
using System.Text;
using App;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StringStatusConverter
{
	public static void SetPlayerStatus(string userData)
	{
		if (userData == "None" || !userData.Contains("="))
		{
			return;
		}
		string[] array = userData.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			if (text[0] == ' ')
			{
				text = text.Substring(1, text.Length - 1);
			}
			if (text[text.Length - 1] == ' ')
			{
				text = text.Substring(0, text.Length - 1);
			}
			SetEachProperty(text);
		}
	}

	public static void SetEachProperty(string key, string value)
	{
		SetEachProperty(key + "=" + value);
	}

	private static void SetEachProperty(string propertyPair)
	{
		string[] array = propertyPair.Split('=');
		string text = array[0];
		string text2 = array[1];
		PropertyInfo propertyInfo = null;
		Type[] playerDataList = AppInfo.PlayerDataList;
		if (playerDataList == null)
		{
			return;
		}
		Type[] array2 = playerDataList;
		foreach (Type type in array2)
		{
			if (AppUtil.HasAttribute(type, typeof(AppUtil.DataRangeAttribute)))
			{
				if (text.Contains(type.Name))
				{
					int num = int.Parse(text.Replace(type.Name, ""));
					string text3 = num.ToString();
					MethodInfo method = type.GetMethod("GetTitle", new Type[1] { typeof(int) });
					if (method != null)
					{
						text3 = method.Invoke(null, new object[1] { num }).ToString();
					}
					method = type.GetMethod("Set", new Type[2]
					{
						typeof(string),
						typeof(int)
					});
					if (method != null)
					{
						method.Invoke(null, new object[2]
						{
							text3,
							int.Parse(text2)
						});
					}
					method = type.GetMethod("Set", new Type[2]
					{
						typeof(string),
						typeof(string)
					});
					if (method != null)
					{
						method.Invoke(null, new object[2] { text3, text2 });
					}
					method = type.GetMethod("Set", new Type[2]
					{
						typeof(string),
						typeof(bool)
					});
					if (method != null)
					{
						method.Invoke(null, new object[2]
						{
							text3,
							bool.Parse(text2)
						});
					}
					method = type.GetMethod("Set", new Type[2]
					{
						typeof(string),
						typeof(TimeSpan)
					});
					if (method != null)
					{
						method.Invoke(null, new object[2]
						{
							text3,
							TimeSpan.Parse(text2)
						});
					}
					return;
				}
				continue;
			}
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo propertyInfo2 in properties)
			{
				if (text == AppUtil.GetTitle(propertyInfo2))
				{
					propertyInfo = propertyInfo2;
					break;
				}
			}
			if (propertyInfo == null)
			{
				propertyInfo = type.GetProperty(text);
			}
			if (propertyInfo != null)
			{
				break;
			}
		}
		if (propertyInfo == null)
		{
			return;
		}
		if (propertyInfo.PropertyType == typeof(float))
		{
			propertyInfo.SetValue(null, float.Parse(text2), null);
			return;
		}
		if (propertyInfo.PropertyType == typeof(int))
		{
			propertyInfo.SetValue(null, int.Parse(text2), null);
			return;
		}
		if (propertyInfo.PropertyType == typeof(bool))
		{
			propertyInfo.SetValue(null, bool.Parse(text2), null);
			return;
		}
		if (propertyInfo.PropertyType == typeof(string))
		{
			propertyInfo.SetValue(null, text2, null);
			return;
		}
		MethodInfo method2 = propertyInfo.PropertyType.GetMethod("CreateInstance");
		if (method2 != null)
		{
			propertyInfo.SetValue(null, method2.Invoke(null, new object[1] { text2 }), null);
		}
	}

	public static string GetStatusString()
	{
		string text = "\n";
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[シーン]" + SceneManager.GetActiveScene().name);
		stringBuilder.Append(text);
		Type[] playerDataList = AppInfo.PlayerDataList;
		if (playerDataList == null)
		{
			return stringBuilder.ToString();
		}
		Type[] array = playerDataList;
		foreach (Type type in array)
		{
			stringBuilder.Append(text + "[" + AppUtil.GetTitle(type) + "]" + text);
			if (AppUtil.HasAttribute(type, typeof(AppUtil.DataRangeAttribute)))
			{
				int num = (int)type.GetProperty("Min").GetValue(null, null);
				int num2 = (int)type.GetProperty("Max").GetValue(null, null);
				MethodInfo method = type.GetMethod("GetTitle");
				MethodInfo method2 = type.GetMethod("GetLabel");
				MethodInfo method3 = type.GetMethod("Get", new Type[1] { typeof(string) });
				for (int j = num; j <= num2; j++)
				{
					string text2 = method.Invoke(null, new object[1] { j }).ToString();
					string value = text2;
					if (method2 != null)
					{
						value = method2.Invoke(null, new object[1] { j }).ToString();
					}
					string value2 = method3.Invoke(null, new object[1] { text2 }).ToString();
					stringBuilder.Append(value);
					stringBuilder.Append("=");
					stringBuilder.Append(value2);
					stringBuilder.Append(text);
				}
			}
			else
			{
				PropertyInfo[] properties = type.GetProperties();
				foreach (PropertyInfo propertyInfo in properties)
				{
					stringBuilder.Append(AppUtil.GetTitle(propertyInfo));
					stringBuilder.Append("=");
					stringBuilder.Append(propertyInfo.GetValue(null, null).ToString());
					stringBuilder.Append(text);
				}
			}
		}
		return stringBuilder.ToString();
	}

	public static string[] GetStatusStringArray(Type[] typeList = null, bool showHideItem = true, bool showReadOnlyItem = true)
	{
		string value = "\t";
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		StringBuilder stringBuilder3 = new StringBuilder();
		if (typeList == null)
		{
			stringBuilder2.Append(SceneManager.GetActiveScene().name);
			stringBuilder2.Append(value);
			stringBuilder3.Append("Scene");
			stringBuilder3.Append(value);
			typeList = AppInfo.PlayerDataList;
		}
		if (typeList != null)
		{
			Type[] array = typeList;
			foreach (Type type in array)
			{
				if (!showHideItem && AppUtil.HasAttribute(type, typeof(AppUtil.HideAttribute)))
				{
					continue;
				}
				if (AppUtil.HasAttribute(type, typeof(AppUtil.DataRangeAttribute)))
				{
					int num = (int)type.GetProperty("Min").GetValue(null, null);
					int num2 = (int)type.GetProperty("Max").GetValue(null, null);
					MethodInfo method = type.GetMethod("GetTitle");
					MethodInfo method2 = type.GetMethod("Get", new Type[1] { typeof(string) });
					for (int j = num; j <= num2; j++)
					{
						string value2 = type.Name + j;
						string text = method.Invoke(null, new object[1] { j }).ToString();
						string value3 = method2.Invoke(null, new object[1] { text }).ToString();
						stringBuilder2.Append(value3);
						stringBuilder2.Append(value);
						stringBuilder3.Append(value2);
						stringBuilder3.Append(value);
					}
					continue;
				}
				PropertyInfo[] properties = type.GetProperties();
				foreach (PropertyInfo propertyInfo in properties)
				{
					if ((showReadOnlyItem || propertyInfo.CanWrite) && (showHideItem || !AppUtil.HasAttribute(propertyInfo, typeof(AppUtil.HideAttribute))))
					{
						string name = propertyInfo.Name;
						stringBuilder2.Append(propertyInfo.GetValue(null, null).ToString());
						stringBuilder2.Append(value);
						stringBuilder3.Append(name);
						stringBuilder3.Append(value);
					}
				}
			}
			return new string[3]
			{
				stringBuilder.ToString(),
				stringBuilder2.ToString(),
				stringBuilder3.ToString()
			};
		}
		return new string[3]
		{
			stringBuilder.ToString(),
			stringBuilder2.ToString(),
			stringBuilder3.ToString()
		};
	}

	public static string GetHierarchyPath(Transform main)
	{
		string text = main.name;
		Transform parent = main.parent;
		while (parent != null)
		{
			text = parent.name + "/" + text;
			parent = parent.parent;
		}
		return text;
	}
}
