using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MiniJSON;
using NCMB.Internal;

namespace NCMB
{
	public class NCMBQuery<T> where T : NCMBObject
	{
		private readonly Dictionary<string, object> _where;

		private readonly string WHERE_URL = "?";

		private string _className;

		private int _limit;

		private int _skip;

		private List<string> _order;

		private List<string> _include;

		public int Skip
		{
			get
			{
				return _skip;
			}
			set
			{
				_skip = value;
			}
		}

		public int Limit
		{
			get
			{
				return _limit;
			}
			set
			{
				_limit = value;
			}
		}

		public string ClassName
		{
			get
			{
				return _className;
			}
		}

		public NCMBQuery(string theClassName)
		{
			_className = theClassName;
			_limit = -1;
			_skip = 0;
			_order = new List<string>();
			_include = new List<string>();
			_where = new Dictionary<string, object>();
		}

		public static NCMBQuery<T> Or(List<NCMBQuery<T>> queries)
		{
			List<NCMBQuery<T>> list = new List<NCMBQuery<T>>();
			string text = null;
			if (queries == null)
			{
				throw new NCMBException(new ArgumentException("queries may not be null."));
			}
			if (queries.Count == 0)
			{
				throw new NCMBException(new ArgumentException("Can't take an or of an empty list of queries"));
			}
			for (int i = 0; i < queries.Count; i++)
			{
				if (text != null && !queries[i]._className.Equals(text))
				{
					throw new NCMBException(new ArgumentException("All of the queries in an or query must be on the same class "));
				}
				text = queries[i]._className;
				list.Add(queries[i]);
			}
			return new NCMBQuery<T>(text)._whereSatifiesAnyOf(list);
		}

		private NCMBQuery<T> _whereSatifiesAnyOf(List<NCMBQuery<T>> queries)
		{
			_where["$or"] = queries;
			return this;
		}

		public NCMBQuery<T> OrderByAscending(string key)
		{
			_order.Clear();
			_order.Add(key);
			return this;
		}

		public NCMBQuery<T> OrderByDescending(string key)
		{
			_order.Clear();
			_order.Add("-" + key);
			return this;
		}

		public NCMBQuery<T> AddAscendingOrder(string key)
		{
			if (_order.Count == 0 || _order[0].Equals(""))
			{
				_order.Clear();
			}
			_order.Add(key);
			return this;
		}

		public NCMBQuery<T> AddDescendingOrder(string key)
		{
			if (_order.Count == 0 || _order[0].Equals(""))
			{
				_order.Clear();
			}
			_order.Add("-" + key);
			return this;
		}

		public NCMBQuery<T> WhereEqualTo(string key, object value)
		{
			value = NCMBUtility._maybeEncodeJSONObject(value, true);
			_where[key] = value;
			return this;
		}

		public NCMBQuery<T> WhereNotEqualTo(string key, object value)
		{
			_addCondition(key, "$ne", value);
			return this;
		}

		public NCMBQuery<T> WhereGreaterThan(string key, object value)
		{
			_addCondition(key, "$gt", value);
			return this;
		}

		public NCMBQuery<T> WhereGreaterThanOrEqualTo(string key, object value)
		{
			_addCondition(key, "$gte", value);
			return this;
		}

		public NCMBQuery<T> WhereLessThan(string key, object value)
		{
			_addCondition(key, "$lt", value);
			return this;
		}

		public NCMBQuery<T> WhereLessThanOrEqualTo(string key, object value)
		{
			_addCondition(key, "$lte", value);
			return this;
		}

		public NCMBQuery<T> WhereContainedIn(string key, IEnumerable values)
		{
			List<object> list = new List<object>();
			foreach (object value in values)
			{
				list.Add(value);
			}
			_addCondition(key, "$in", list);
			return this;
		}

		public NCMBQuery<T> WhereNotContainedIn(string key, IEnumerable values)
		{
			List<object> list = new List<object>();
			foreach (object value in values)
			{
				list.Add(value);
			}
			_addCondition(key, "$nin", list);
			return this;
		}

		public NCMBQuery<T> WhereContainedInArray(string key, IEnumerable values)
		{
			List<object> list = new List<object>();
			foreach (object value in values)
			{
				list.Add(value);
			}
			_addCondition(key, "$inArray", list);
			return this;
		}

		public NCMBQuery<T> WhereContainsAll(string key, IEnumerable values)
		{
			List<object> list = new List<object>();
			foreach (object value in values)
			{
				list.Add(value);
			}
			_addCondition(key, "$all", list);
			return this;
		}

		public NCMBQuery<T> WhereMatchesQuery<TOther>(string key, NCMBQuery<TOther> query) where TOther : NCMBObject
		{
			_addCondition(key, "$inQuery", query);
			return this;
		}

		public NCMBQuery<T> WhereMatchesKeyInQuery<TOther>(string mainKey, string subKey, NCMBQuery<TOther> query) where TOther : NCMBObject
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["query"] = query;
			dictionary["key"] = subKey;
			_addCondition(mainKey, "$select", dictionary);
			return this;
		}

		public void Include(string key)
		{
			_include.Add(key);
		}

		public NCMBQuery<T> WhereNearGeoPoint(string key, NCMBGeoPoint point)
		{
			object value = _geoPointToObject(point);
			_addCondition(key, "$nearSphere", value);
			return this;
		}

		public NCMBQuery<T> WhereGeoPointWithinKilometers(string key, NCMBGeoPoint point, double maxDistance)
		{
			Dictionary<string, object> value = _geoPointToObject(point);
			_addCondition(key, "$nearSphere", value);
			_addCondition(key, "$maxDistanceInKilometers", maxDistance);
			return this;
		}

		public NCMBQuery<T> WhereGeoPointWithinMiles(string key, NCMBGeoPoint point, double maxDistance)
		{
			Dictionary<string, object> value = _geoPointToObject(point);
			_addCondition(key, "$nearSphere", value);
			_addCondition(key, "$maxDistanceInMiles", maxDistance);
			return this;
		}

		public NCMBQuery<T> WhereGeoPointWithinRadians(string key, NCMBGeoPoint point, double maxDistance)
		{
			Dictionary<string, object> value = _geoPointToObject(point);
			_addCondition(key, "$nearSphere", value);
			_addCondition(key, "$maxDistanceInRadians", maxDistance);
			return this;
		}

		public NCMBQuery<T> WhereWithinGeoBox(string key, NCMBGeoPoint southwest, NCMBGeoPoint northeast)
		{
			Dictionary<string, object> value = _geoPointToObjectWithinBox(southwest, northeast);
			_addCondition(key, "$within", value);
			return this;
		}

		private Dictionary<string, object> _geoPointToObjectWithinBox(NCMBGeoPoint southwest, NCMBGeoPoint northeast)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> item = _geoPointToObject(southwest);
			Dictionary<string, object> item2 = _geoPointToObject(northeast);
			dictionary["$box"] = new List<object> { item, item2 };
			return dictionary;
		}

		private Dictionary<string, object> _geoPointToObject(NCMBGeoPoint point)
		{
			return new Dictionary<string, object>
			{
				{ "__type", "GeoPoint" },
				{ "longitude", point.Longitude },
				{ "latitude", point.Latitude }
			};
		}

		private void _addCondition(string key, string condition, object value)
		{
			Dictionary<string, object> dictionary = null;
			value = NCMBUtility._maybeEncodeJSONObject(value, true);
			if (_where.ContainsKey(key))
			{
				object obj = _where[key];
				if (obj is IDictionary)
				{
					dictionary = (Dictionary<string, object>)obj;
				}
			}
			if (dictionary == null)
			{
				dictionary = new Dictionary<string, object>();
			}
			dictionary[condition] = value;
			_where[key] = dictionary;
		}

		public void FindAsync(NCMBQueryCallback<T> callback)
		{
			if (callback == null)
			{
				throw new ArgumentException("It is necessary to always set a callback.");
			}
			Find(callback);
		}

		internal void Find(NCMBQueryCallback<T> callback)
		{
			string text = _getSearchUrl(_className);
			text += WHERE_URL;
			Dictionary<string, object> beforejsonData = _getFindParams();
			text = _makeWhereUrl(text, beforejsonData);
			ConnectType method = ConnectType.GET;
			new NCMBConnection(text, method, null, NCMBUser._getCurrentSessionToken()).Connect(delegate(int statusCode, string responseData, NCMBException error)
			{
				List<T> list = new List<T>();
				try
				{
					if (error == null)
					{
						Dictionary<string, object> response = Json.Deserialize(responseData) as Dictionary<string, object>;
						foreach (T item in _convertFindResponse(response))
						{
							list.Add(item);
						}
					}
				}
				catch (Exception error2)
				{
					error = new NCMBException(error2);
				}
				callback(list, error);
			});
		}

		private ArrayList _convertFindResponse(Dictionary<string, object> response)
		{
			ArrayList arrayList = new ArrayList();
			List<object> list = (List<object>)response["results"];
			if (list == null)
			{
				Debug.Log("null results in find response");
			}
			else
			{
				object value = null;
				string text = null;
				if (response.TryGetValue("className", out value))
				{
					text = (string)value;
				}
				if (text == null)
				{
					text = _className;
				}
				for (int i = 0; i < list.Count; i++)
				{
					NCMBObject nCMBObject = null;
					nCMBObject = ((!text.Equals("user")) ? ((!text.Equals("role")) ? ((!text.Equals("installation")) ? ((!text.Equals("push")) ? ((!text.Equals("file")) ? new NCMBObject(text) : new NCMBFile()) : new NCMBPush()) : new NCMBInstallation()) : new NCMBRole()) : new NCMBUser());
					nCMBObject._mergeFromServer((Dictionary<string, object>)list[i], true);
					arrayList.Add(nCMBObject);
				}
			}
			return arrayList;
		}

		public void GetAsync(string objectId, NCMBGetCallback<T> callback)
		{
			if (callback == null)
			{
				throw new ArgumentException("It is necessary to always set a callback.");
			}
			string url = _getSearchUrl(_className) + "/" + objectId;
			ConnectType method = ConnectType.GET;
			new NCMBConnection(url, method, null, NCMBUser._getCurrentSessionToken()).Connect(delegate(int statusCode, string responseData, NCMBException error)
			{
				NCMBObject nCMBObject = null;
				try
				{
					if (error == null)
					{
						Dictionary<string, object> response = Json.Deserialize(responseData) as Dictionary<string, object>;
						nCMBObject = _convertGetResponse(response);
					}
				}
				catch (Exception error2)
				{
					error = new NCMBException(error2);
				}
				callback((T)nCMBObject, error);
			});
		}

		private NCMBObject _convertGetResponse(Dictionary<string, object> response)
		{
			NCMBObject result = null;
			if (response == null)
			{
				Debug.Log("null results in get response");
			}
			else
			{
				string className = _className;
				NCMBObject nCMBObject = null;
				nCMBObject = ((!className.Equals("user")) ? new NCMBObject(className) : new NCMBUser());
				nCMBObject._mergeFromServer(response, true);
				result = nCMBObject;
			}
			return result;
		}

		public void CountAsync(NCMBCountCallback callback)
		{
			if (callback == null)
			{
				throw new ArgumentException("It is necessary to always set a callback.");
			}
			string text = _getSearchUrl(_className);
			text += WHERE_URL;
			Dictionary<string, object> dictionary = _getFindParams();
			dictionary["count"] = 1;
			text = _makeWhereUrl(text, dictionary);
			ConnectType method = ConnectType.GET;
			new NCMBConnection(text, method, null, NCMBUser._getCurrentSessionToken()).Connect(delegate(int statusCode, string responseData, NCMBException error)
			{
				int count = 0;
				if (error == null)
				{
					try
					{
						Dictionary<string, object> obj = Json.Deserialize(responseData) as Dictionary<string, object>;
						object value = null;
						if (obj.TryGetValue("count", out value))
						{
							count = Convert.ToInt32(value);
						}
					}
					catch (Exception error2)
					{
						error = new NCMBException(error2);
					}
				}
				callback(count, error);
			});
		}

		private string _makeWhereUrl(string url, Dictionary<string, object> beforejsonData)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(url);
			foreach (string key in beforejsonData.Keys)
			{
				if (!key.Equals("className"))
				{
					string stringToEscape = ((beforejsonData[key] is IDictionary) ? Json.Serialize((Dictionary<string, object>)beforejsonData[key]) : ((!(beforejsonData[key] is int)) ? ((string)beforejsonData[key]) : Json.Serialize((int)beforejsonData[key])));
					string text = Uri.EscapeUriString(stringToEscape);
					text = text.Replace(":", "%3A");
					stringBuilder.Append(key).Append("=").Append(text)
						.Append("&");
				}
			}
			if (beforejsonData.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}

		private Dictionary<string, object> _getFindParams()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary["className"] = _className;
			foreach (string key in _where.Keys)
			{
				if (key.Equals("$or"))
				{
					List<NCMBQuery<T>> obj = (List<NCMBQuery<T>>)_where[key];
					List<object> list = new List<object>();
					foreach (NCMBQuery<T> item in obj)
					{
						if (item._limit >= 0)
						{
							throw new ArgumentException("Cannot have limits in sub queries of an 'OR' query");
						}
						if (item._skip > 0)
						{
							throw new ArgumentException("Cannot have skips in sub queries of an 'OR' query");
						}
						if (item._order.Count > 0)
						{
							throw new ArgumentException("Cannot have an order in sub queries of an 'OR' query");
						}
						if (item._include.Count > 0)
						{
							throw new ArgumentException("Cannot have an include in sub queries of an 'OR' query");
						}
						Dictionary<string, object> dictionary3 = item._getFindParams();
						if (dictionary3["where"] != null)
						{
							list.Add(dictionary3["where"]);
						}
						else
						{
							list.Add(new Dictionary<string, object>());
						}
					}
					dictionary2[key] = list;
				}
				else
				{
					object value = _encodeSubQueries(_where[key]);
					dictionary2[key] = NCMBUtility._maybeEncodeJSONObject(value, true);
				}
			}
			dictionary["where"] = dictionary2;
			if (_limit >= 0)
			{
				dictionary["limit"] = _limit;
			}
			if (_skip > 0)
			{
				dictionary["skip"] = _skip;
			}
			if (_order.Count > 0)
			{
				dictionary["order"] = _join(_order, ",");
			}
			if (_include.Count > 0)
			{
				dictionary["include"] = _join(_include, ",");
			}
			return dictionary;
		}

		private object _encodeSubQueries(object value)
		{
			if (!(value is IDictionary))
			{
				return value;
			}
			Dictionary<string, object> obj = (Dictionary<string, object>)value;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> item in obj)
			{
				if (item.Value is NCMBQuery<NCMBObject>)
				{
					Dictionary<string, object> dictionary2 = ((NCMBQuery<NCMBObject>)item.Value)._getFindParams();
					dictionary2["where"] = dictionary2["where"];
					dictionary[item.Key] = dictionary2;
				}
				else if (item.Value is NCMBQuery<NCMBUser>)
				{
					Dictionary<string, object> dictionary3 = ((NCMBQuery<NCMBUser>)item.Value)._getFindParams();
					dictionary3["where"] = dictionary3["where"];
					dictionary[item.Key] = dictionary3;
				}
				else if (item.Value is NCMBQuery<NCMBRole>)
				{
					Dictionary<string, object> dictionary4 = ((NCMBQuery<NCMBRole>)item.Value)._getFindParams();
					dictionary4["where"] = dictionary4["where"];
					dictionary[item.Key] = dictionary4;
				}
				else if (item.Value is NCMBQuery<NCMBInstallation>)
				{
					Dictionary<string, object> dictionary5 = ((NCMBQuery<NCMBInstallation>)item.Value)._getFindParams();
					dictionary5["where"] = dictionary5["where"];
					dictionary[item.Key] = dictionary5;
				}
				else if (item.Value is NCMBQuery<NCMBPush>)
				{
					Dictionary<string, object> dictionary6 = ((NCMBQuery<NCMBPush>)item.Value)._getFindParams();
					dictionary6["where"] = dictionary6["where"];
					dictionary[item.Key] = dictionary6;
				}
				else if (item.Value is IDictionary)
				{
					dictionary[item.Key] = _encodeSubQueries(item.Value);
				}
				else
				{
					dictionary[item.Key] = item.Value;
				}
			}
			return dictionary;
		}

		private string _join(List<string> items, string delimiter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in items)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(delimiter);
				}
				stringBuilder.Append(item);
			}
			return stringBuilder.ToString();
		}

		private string _getSearchUrl(string className)
		{
			string text = "";
			if (className == null || className.Equals(""))
			{
				throw new ArgumentException("Not class name error. Please be sure to specify the class name.");
			}
			if (className.Equals("push"))
			{
				return new NCMBPush()._getBaseUrl();
			}
			if (className.Equals("installation"))
			{
				return new NCMBInstallation()._getBaseUrl();
			}
			if (className.Equals("file"))
			{
				return new NCMBFile()._getBaseUrl();
			}
			if (className.Equals("user"))
			{
				return new NCMBUser()._getBaseUrl();
			}
			if (className.Equals("role"))
			{
				return new NCMBRole()._getBaseUrl();
			}
			return new NCMBObject(_className)._getBaseUrl();
		}

		public static NCMBQuery<T> GetQuery(string className)
		{
			return new NCMBQuery<T>(className);
		}

		internal NCMBQuery<T> _whereRelatedTo(NCMBObject parent, string key)
		{
			_addCondition("$relatedTo", "object", NCMBUtility._maybeEncodeJSONObject(parent, true));
			_addCondition("$relatedTo", "key", key);
			return this;
		}
	}
}
