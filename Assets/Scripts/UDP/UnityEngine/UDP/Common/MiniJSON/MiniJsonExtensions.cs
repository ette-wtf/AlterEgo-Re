using System;
using System.Collections.Generic;

namespace UnityEngine.UDP.Common.MiniJSON
{
	public static class MiniJsonExtensions
	{
		public static Dictionary<string, object> GetHash(this Dictionary<string, object> dic, string key)
		{
			return (Dictionary<string, object>)dic[key];
		}

		public static T GetEnum<T>(this Dictionary<string, object> dic, string key)
		{
			if (dic.ContainsKey(key))
			{
				return (T)Enum.Parse(typeof(T), dic[key].ToString(), true);
			}
			return default(T);
		}

		public static string GetString(this Dictionary<string, object> dic, string key, string defaultValue = "")
		{
			if (dic.ContainsKey(key))
			{
				return dic[key].ToString();
			}
			return defaultValue;
		}

		public static long GetLong(this Dictionary<string, object> dic, string key)
		{
			if (dic.ContainsKey(key))
			{
				return long.Parse(dic[key].ToString());
			}
			return 0L;
		}

		public static List<string> GetStringList(this Dictionary<string, object> dic, string key)
		{
			if (dic.ContainsKey(key))
			{
				List<string> list = new List<string>();
				{
					foreach (object item in (List<object>)dic[key])
					{
						list.Add(item.ToString());
					}
					return list;
				}
			}
			return new List<string>();
		}

		public static bool GetBool(this Dictionary<string, object> dic, string key)
		{
			if (dic.ContainsKey(key))
			{
				return bool.Parse(dic[key].ToString());
			}
			return false;
		}

		public static T Get<T>(this Dictionary<string, object> dic, string key)
		{
			if (dic.ContainsKey(key))
			{
				return (T)dic[key];
			}
			return default(T);
		}

		public static string toJson(this Dictionary<string, object> obj)
		{
			return MiniJson.JsonEncode(obj);
		}

		public static string toJson(this Dictionary<string, string> obj)
		{
			return MiniJson.JsonEncode(obj);
		}

		public static string toJson(this string[] array)
		{
			List<object> list = new List<object>();
			foreach (string item in array)
			{
				list.Add(item);
			}
			return MiniJson.JsonEncode(list);
		}

		public static Dictionary<string, object> HashtableFromJson(this string json)
		{
			return MiniJson.JsonDecode(json);
		}
	}
}
