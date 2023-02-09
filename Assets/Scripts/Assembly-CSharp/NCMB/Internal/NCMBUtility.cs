using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NCMB.Internal
{
	internal static class NCMBUtility
	{
		internal static string GetClassName(object t)
		{
			try
			{
				NCMBClassNameAttribute nCMBClassNameAttribute = (NCMBClassNameAttribute)t.GetType().GetCustomAttributes(true)[0];
				if (nCMBClassNameAttribute == null || nCMBClassNameAttribute.ClassName == null)
				{
					throw new NCMBException(new ArgumentException("No ClassName attribute specified on the given subclass."));
				}
				return nCMBClassNameAttribute.ClassName;
			}
			catch (Exception error)
			{
				throw new NCMBException(error);
			}
		}

		internal static void CopyDictionary(IDictionary<string, object> listSouce, IDictionary<string, object> listDestination)
		{
			try
			{
				foreach (KeyValuePair<string, object> item in listSouce)
				{
					listDestination[item.Key] = item.Value;
				}
			}
			catch (Exception error)
			{
				throw new NCMBException(error);
			}
		}

		private static IDictionary<string, object> _encodeJSONObject(object value, bool allowNCMBObjects)
		{
			if (value is DateTime)
			{
				DateTime dateTime = (DateTime)value;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				string value2 = dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
				dictionary.Add("__type", "Date");
				dictionary.Add("iso", value2);
				return dictionary;
			}
			if (value is NCMBObject)
			{
				NCMBObject nCMBObject = (NCMBObject)value;
				if (!allowNCMBObjects)
				{
					throw new ArgumentException("NCMBObjects not allowed here.");
				}
				if (nCMBObject.ObjectId == null)
				{
					throw new ArgumentException("Cannot create a pointer to an object without an objectId.");
				}
				return new Dictionary<string, object>
				{
					{ "__type", "Pointer" },
					{ "className", nCMBObject.ClassName },
					{ "objectId", nCMBObject.ObjectId }
				};
			}
			if (value is NCMBGeoPoint)
			{
				NCMBGeoPoint nCMBGeoPoint = (NCMBGeoPoint)value;
				return new Dictionary<string, object>
				{
					{ "__type", "GeoPoint" },
					{ "latitude", nCMBGeoPoint.Latitude },
					{ "longitude", nCMBGeoPoint.Longitude }
				};
			}
			if (value is IDictionary)
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				IDictionary dictionary3 = (IDictionary)value;
				{
					foreach (object key in dictionary3.Keys)
					{
						if (key is string)
						{
							dictionary2[(string)key] = _maybeEncodeJSONObject(dictionary3[key], allowNCMBObjects);
							continue;
						}
						throw new NCMBException(new ArgumentException("Invalid type for key: " + key.GetType().ToString() + ".key type string only."));
					}
					return dictionary2;
				}
			}
			if (value is NCMBRelation<NCMBObject>)
			{
				return ((NCMBRelation<NCMBObject>)value)._encodeToJSON();
			}
			if (value is NCMBACL)
			{
				return ((NCMBACL)value)._toJSONObject();
			}
			return null;
		}

		internal static List<object> _encodeAsJSONArray(IList list, bool allowNCMBObjects)
		{
			List<object> list2 = new List<object>();
			foreach (object item in list)
			{
				if (!NCMBObject._isValidType(item))
				{
					throw new NCMBException(new ArgumentException("invalid type for value in array: " + item.GetType().ToString()));
				}
				list2.Add(_maybeEncodeJSONObject(item, allowNCMBObjects));
			}
			return list2;
		}

		internal static object _maybeEncodeJSONObject(object value, bool allowNCMBObjects)
		{
			if (value is INCMBFieldOperation)
			{
				return ((INCMBFieldOperation)value).Encode();
			}
			if (value is IList)
			{
				return _encodeAsJSONArray((IList)value, allowNCMBObjects);
			}
			IDictionary<string, object> dictionary = _encodeJSONObject(value, allowNCMBObjects);
			if (dictionary != null)
			{
				return dictionary;
			}
			return value;
		}

		internal static object decodeJSONObject(object jsonDicParameter)
		{
			if (jsonDicParameter is IList)
			{
				ArrayList arrayList = new ArrayList();
				List<object> list = new List<object>();
				list = (List<object>)jsonDicParameter;
				for (int i = 0; i < list.Count; i++)
				{
					if (decodeJSONObject(list[i]) != null)
					{
						arrayList.Add(decodeJSONObject(list[i]));
					}
					else
					{
						arrayList.Add(list[i]);
					}
				}
				return arrayList;
			}
			if (jsonDicParameter is IDictionary)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)jsonDicParameter;
				object value;
				dictionary.TryGetValue("__type", out value);
				if (value == null)
				{
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					{
						foreach (KeyValuePair<string, object> item in dictionary)
						{
							object obj = decodeJSONObject(item.Value);
							if (obj != null)
							{
								dictionary2.Add(item.Key, obj);
							}
							else
							{
								dictionary2.Add(item.Key, item.Value);
							}
						}
						return dictionary2;
					}
				}
				if (value.Equals("Date"))
				{
					object value2;
					dictionary.TryGetValue("iso", out value2);
					return parseDate((string)value2);
				}
				if (value.Equals("Pointer"))
				{
					object value3;
					dictionary.TryGetValue("className", out value3);
					object value4;
					dictionary.TryGetValue("objectId", out value4);
					return NCMBObject.CreateWithoutData((string)value3, (string)value4);
				}
				if (value.Equals("GeoPoint"))
				{
					double num = 0.0;
					double num2 = 0.0;
					try
					{
						object value5;
						dictionary.TryGetValue("latitude", out value5);
						num = Convert.ToDouble(value5);
						object value6;
						dictionary.TryGetValue("longitude", out value6);
						num2 = Convert.ToDouble(value6);
					}
					catch (Exception error)
					{
						throw new NCMBException(error);
					}
					return new NCMBGeoPoint(num, num2);
				}
				if (value.Equals("Object"))
				{
					object value7;
					dictionary.TryGetValue("className", out value7);
					NCMBObject nCMBObject = NCMBObject.CreateWithoutData((string)value7, null);
					nCMBObject._handleFetchResult(true, dictionary);
					return nCMBObject;
				}
				if (value.Equals("Relation"))
				{
					if (dictionary["className"].Equals("user"))
					{
						return new NCMBRelation<NCMBUser>((string)dictionary["className"]);
					}
					if (dictionary["className"].Equals("role"))
					{
						return new NCMBRelation<NCMBRole>((string)dictionary["className"]);
					}
					return new NCMBRelation<NCMBObject>((string)dictionary["className"]);
				}
				return null;
			}
			return null;
		}

		internal static DateTime parseDate(string dateString)
		{
			string format = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";
			return DateTime.ParseExact(dateString, format, null);
		}

		internal static string encodeDate(DateTime dateObject)
		{
			return dateObject.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
		}

		private static bool isContainerObject(object object1)
		{
			if (!(object1 is NCMBGeoPoint))
			{
				return object1 is IDictionary;
			}
			return true;
		}

		internal static string _encodeString(string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			char[] array = str.ToCharArray();
			foreach (char c in array)
			{
				switch (c)
				{
				case '"':
					stringBuilder.Append("\\\"");
					continue;
				case '\\':
					stringBuilder.Append("\\\\");
					continue;
				case '\b':
					stringBuilder.Append("\\b");
					continue;
				case '\f':
					stringBuilder.Append("\\f");
					continue;
				case '\n':
					stringBuilder.Append("\\n");
					continue;
				case '\r':
					stringBuilder.Append("\\r");
					continue;
				case '\t':
					stringBuilder.Append("\\t");
					continue;
				}
				int num = Convert.ToInt32(c);
				if (num >= 32 && num <= 126)
				{
					stringBuilder.Append(c);
					continue;
				}
				stringBuilder.Append("\\u");
				stringBuilder.Append(num.ToString("x4"));
			}
			return stringBuilder.ToString();
		}

		internal static string unicodeUnescape(string targetText)
		{
			return Regex.Replace(targetText, "\\\\[Uu]([0-9A-Fa-f]{4})", (Match x) => ((char)ushort.Parse(x.Groups[1].Value, NumberStyles.AllowHexSpecifier)).ToString()).ToString();
		}
	}
}
