using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using MiniJSON;
using NCMB.Internal;

namespace NCMB
{
	public class NCMBObject
	{
		private static readonly string AUTO_CLASS_NAME = "_NewAutoClass";

		private bool _dirty;

		private readonly IDictionary<string, bool> dataAvailability = new Dictionary<string, bool>();

		internal IDictionary<string, object> estimatedData = new Dictionary<string, object>();

		internal IDictionary<string, object> serverData = new Dictionary<string, object>();

		internal readonly object mutex = new object();

		internal static readonly object fileMutex = new object();

		private readonly LinkedList<IDictionary<string, INCMBFieldOperation>> operationSetQueue = new LinkedList<IDictionary<string, INCMBFieldOperation>>();

		private string _className;

		private string _objectId;

		private DateTime? _updateDate;

		private DateTime? _createDate;

		public virtual object this[string key]
		{
			get
			{
				lock (mutex)
				{
					_checkGetAccess(key);
					if (!estimatedData.ContainsKey(key))
					{
						throw new NCMBException(new ArgumentException("The given key was not present in the dictionary."));
					}
					object obj = estimatedData[key];
					if (obj is NCMBRelation<NCMBObject>)
					{
						((NCMBRelation<NCMBObject>)obj)._ensureParentAndKey(this, key);
					}
					else if (obj is NCMBRelation<NCMBUser>)
					{
						((NCMBRelation<NCMBUser>)obj)._ensureParentAndKey(this, key);
					}
					else if (obj is NCMBRelation<NCMBRole>)
					{
						((NCMBRelation<NCMBRole>)obj)._ensureParentAndKey(this, key);
					}
					return obj;
				}
			}
			set
			{
				lock (mutex)
				{
					_onSettingValue(key, value);
					if (!_isValidType(value))
					{
						throw new NCMBException(new ArgumentException("Invalid type for value: " + value.GetType().ToString()));
					}
					estimatedData[key] = value;
					_performOperation(key, new NCMBSetOperation(value));
				}
			}
		}

		public string ClassName
		{
			get
			{
				return _className;
			}
			private set
			{
				_className = value;
			}
		}

		public string ObjectId
		{
			get
			{
				return _objectId;
			}
			set
			{
				lock (mutex)
				{
					_dirty = true;
					_objectId = value;
				}
			}
		}

		public DateTime? UpdateDate
		{
			get
			{
				return _updateDate;
			}
			private set
			{
				_updateDate = value;
			}
		}

		public DateTime? CreateDate
		{
			get
			{
				return _createDate;
			}
			set
			{
				_createDate = value;
			}
		}

		public NCMBACL ACL
		{
			get
			{
				object value = null;
				estimatedData.TryGetValue("acl", out value);
				if (value == null)
				{
					return null;
				}
				if (!(value is NCMBACL))
				{
					throw new NCMBException(new ArgumentException("only ACLs can be stored in the ACL key"));
				}
				NCMBACL nCMBACL = (NCMBACL)value;
				if (nCMBACL._isShared())
				{
					NCMBACL nCMBACL2 = nCMBACL._copy();
					estimatedData["acl"] = nCMBACL2;
					return nCMBACL2;
				}
				return nCMBACL;
			}
			set
			{
				this["acl"] = value;
			}
		}

		public bool IsDirty
		{
			get
			{
				lock (mutex)
				{
					return _checkIsDirty(true);
				}
			}
			internal set
			{
				lock (mutex)
				{
					_dirty = value;
				}
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				lock (mutex)
				{
					return estimatedData.Keys;
				}
			}
		}

		internal IDictionary<string, INCMBFieldOperation> _currentOperations
		{
			get
			{
				return operationSetQueue.Last.Value;
			}
		}

		internal virtual void _onSettingValue(string key, object value)
		{
			if (key == null)
			{
				throw new NCMBException(new ArgumentNullException("key"));
			}
		}

		private bool _checkIsDataAvailable(string key)
		{
			if (dataAvailability.ContainsKey(key))
			{
				return dataAvailability[key];
			}
			return false;
		}

		private void _checkGetAccess(string key)
		{
			if (!_checkIsDataAvailable(key))
			{
				throw new NCMBException(new InvalidOperationException("NCMBObject has no data for this key.  Call FetchAsync() to get the data."));
			}
		}

		private bool _checkIsDirty(bool considerChildren)
		{
			if (!_dirty && _currentOperations.Count <= 0)
			{
				if (considerChildren)
				{
					return _hasDirtyChildren();
				}
				return false;
			}
			return true;
		}

		private bool _hasDirtyChildren()
		{
			lock (mutex)
			{
				List<NCMBObject> list = new List<NCMBObject>();
				_findUnsavedChildren(estimatedData, list);
				return list.Count > 0;
			}
		}

		private static void _findUnsavedChildren(object data, List<NCMBObject> unsaved)
		{
			if (data is IList)
			{
				foreach (object item in (IList)data)
				{
					_findUnsavedChildren(item, unsaved);
				}
				return;
			}
			if (data is IDictionary)
			{
				foreach (object value in ((IDictionary)data).Values)
				{
					_findUnsavedChildren(value, unsaved);
				}
				return;
			}
			if (data is NCMBObject)
			{
				NCMBObject nCMBObject = (NCMBObject)data;
				if (nCMBObject.IsDirty)
				{
					unsaved.Add(nCMBObject);
				}
			}
		}

		internal NCMBObject()
			: this(AUTO_CLASS_NAME)
		{
		}

		public NCMBRelation<T> GetRelation<T>(string key) where T : NCMBObject
		{
			NCMBRelation<T> nCMBRelation = new NCMBRelation<T>(this, key);
			object value = null;
			estimatedData.TryGetValue(key, out value);
			if (value is NCMBRelation<T>)
			{
				nCMBRelation.TargetClass = ((NCMBRelation<T>)value).TargetClass;
			}
			return nCMBRelation;
		}

		public NCMBObject(string className)
		{
			if (className == "" || className == null)
			{
				throw new NCMBException("You must specify class name or invalid classname");
			}
			if (className == AUTO_CLASS_NAME)
			{
				ClassName = NCMBUtility.GetClassName(this);
			}
			else
			{
				ClassName = className;
			}
			operationSetQueue.AddLast(new Dictionary<string, INCMBFieldOperation>());
			estimatedData = new Dictionary<string, object>();
			IsDirty = true;
			dataAvailability = new Dictionary<string, bool>();
			_setDefaultValues();
		}

		internal void _performOperation(string key, INCMBFieldOperation operation)
		{
			lock (mutex)
			{
				try
				{
					object value;
					estimatedData.TryGetValue(key, out value);
					object obj = operation.Apply(value, this, key);
					if (obj != null)
					{
						estimatedData[key] = obj;
					}
					else
					{
						estimatedData.Remove(key);
					}
					INCMBFieldOperation value2;
					_currentOperations.TryGetValue(key, out value2);
					INCMBFieldOperation value3 = operation.MergeWithPrevious(value2);
					_currentOperations[key] = value3;
					dataAvailability[key] = true;
				}
				catch (Exception error)
				{
					throw new NCMBException(error);
				}
			}
		}

		internal IDictionary<string, INCMBFieldOperation> StartSave()
		{
			object obj;
			Monitor.Enter(obj = mutex);
			IDictionary<string, INCMBFieldOperation> dictionary = null;
			try
			{
				IDictionary<string, INCMBFieldOperation> currentOperations = _currentOperations;
				operationSetQueue.AddLast(new Dictionary<string, INCMBFieldOperation>());
				return currentOperations;
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		internal static bool _isValidType(object value)
		{
			if (value != null && !(value is string) && !(value is NCMBObject) && !(value is NCMBGeoPoint) && !(value is DateTime) && !(value is IDictionary) && !(value is IList) && !(value is NCMBACL) && !value.GetType().IsPrimitive())
			{
				return value is NCMBRelation<NCMBObject>;
			}
			return true;
		}

		internal void _listIsValidType(IEnumerable values)
		{
			IEnumerator enumerator = values.GetEnumerator();
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				if (!_isValidType(current))
				{
					throw new NCMBException(new ArgumentException("invalid type for value: " + current.GetType().ToString()));
				}
			}
		}

		public void Revert()
		{
			if (_currentOperations.Count > 0)
			{
				_currentOperations.Clear();
				operationSetQueue.Clear();
				operationSetQueue.AddLast(new Dictionary<string, INCMBFieldOperation>());
				_rebuildEstimatedData();
				_dirty = false;
			}
		}

		private void _rebuildEstimatedData()
		{
			lock (mutex)
			{
				estimatedData.Clear();
				foreach (KeyValuePair<string, object> serverDatum in serverData)
				{
					estimatedData.Add(serverDatum);
				}
			}
		}

		private void _updateLatestEstimatedData()
		{
			lock (mutex)
			{
				if (operationSetQueue.Count <= 0)
				{
					return;
				}
				foreach (IDictionary<string, INCMBFieldOperation> item in operationSetQueue)
				{
					_applyOperations(item, estimatedData);
				}
			}
		}

		private void _applyOperations(IDictionary<string, INCMBFieldOperation> operations, IDictionary<string, object> map)
		{
			lock (mutex)
			{
				foreach (KeyValuePair<string, INCMBFieldOperation> operation in operations)
				{
					object value;
					map.TryGetValue(operation.Key, out value);
					object obj = operation.Value.Apply(value, this, operation.Key);
					if (obj != null)
					{
						map[operation.Key] = obj;
					}
					else
					{
						map.Remove(operation.Key);
					}
				}
			}
		}

		public virtual void Add(string key, object value)
		{
			lock (mutex)
			{
				if (key == null)
				{
					throw new NCMBException(new ArgumentException("key may not be null."));
				}
				if (value == null)
				{
					throw new NCMBException(new ArgumentException("value may not be null."));
				}
				if (estimatedData.ContainsKey(key))
				{
					throw new NCMBException(new ArgumentException("Key already exists", key));
				}
				this[key] = value;
			}
		}

		public virtual void Remove(string key)
		{
			lock (mutex)
			{
				object value;
				if (estimatedData.TryGetValue(key, out value))
				{
					estimatedData.Remove(key);
					_currentOperations[key] = new NCMBDeleteOperation();
				}
			}
		}

		public void RemoveRangeFromList(string key, IEnumerable values)
		{
			_listIsValidType(values);
			if (_objectId == null || _objectId.Equals(""))
			{
				IList list = null;
				object value;
				if (!estimatedData.TryGetValue(key, out value))
				{
					throw new NCMBException(new ArgumentException("Does not have a value."));
				}
				if (!(value is IList))
				{
					throw new NCMBException(new ArgumentException("Old value is not an array."));
				}
				list = new ArrayList((IList)value);
				ArrayList arrayList = new ArrayList(list);
				foreach (object value2 in values)
				{
					while (arrayList.Contains(value2))
					{
						arrayList.Remove(value2);
					}
				}
				ArrayList arrayList2 = new ArrayList((IList)values);
				foreach (object item in arrayList)
				{
					while (arrayList2.Contains(item))
					{
						arrayList2.Remove(item);
					}
				}
				HashSet<object> hashSet = new HashSet<object>();
				foreach (object item2 in arrayList2)
				{
					if (item2 is NCMBObject)
					{
						NCMBObject nCMBObject = (NCMBObject)item2;
						hashSet.Add(nCMBObject.ObjectId);
					}
					for (int i = 0; i < arrayList.Count; i++)
					{
						object obj = arrayList[i];
						if (obj is NCMBObject)
						{
							NCMBObject nCMBObject2 = (NCMBObject)obj;
							if (hashSet.Contains(nCMBObject2.ObjectId))
							{
								arrayList.RemoveAt(i);
							}
						}
					}
				}
				NCMBSetOperation operation = new NCMBSetOperation(arrayList);
				_performOperation(key, operation);
			}
			else
			{
				NCMBRemoveOperation operation2 = new NCMBRemoveOperation(values);
				_performOperation(key, operation2);
			}
		}

		public void AddToList(string key, object value)
		{
			AddRangeToList(key, new ArrayList { value });
		}

		public void AddRangeToList(string key, IEnumerable values)
		{
			_listIsValidType(values);
			if (_objectId == null || _objectId.Equals(""))
			{
				ArrayList arrayList = null;
				object value;
				if (estimatedData.TryGetValue(key, out value))
				{
					if (!(value is IList))
					{
						throw new NCMBException(new ArgumentException("Old value is not an array."));
					}
					arrayList = new ArrayList((IList)value);
					IEnumerator enumerator = values.GetEnumerator();
					while (enumerator.MoveNext())
					{
						object current = enumerator.Current;
						arrayList.Add(current);
					}
					NCMBSetOperation operation = new NCMBSetOperation(arrayList);
					_performOperation(key, operation);
				}
				else
				{
					NCMBSetOperation operation2 = new NCMBSetOperation(values);
					_performOperation(key, operation2);
				}
			}
			else
			{
				NCMBAddOperation operation3 = new NCMBAddOperation(values);
				_performOperation(key, operation3);
			}
		}

		public void AddUniqueToList(string key, object value)
		{
			AddRangeUniqueToList(key, new ArrayList { value });
		}

		public void AddRangeUniqueToList(string key, IEnumerable values)
		{
			_listIsValidType(values);
			if (_objectId == null || _objectId.Equals(""))
			{
				ArrayList arrayList = null;
				object value;
				if (estimatedData.TryGetValue(key, out value))
				{
					if (!(value is IList))
					{
						throw new NCMBException(new ArgumentException("Old value is not an array."));
					}
					arrayList = new ArrayList((IList)value);
					Hashtable hashtable = new Hashtable();
					foreach (object item in arrayList)
					{
						int num = 0;
						if (item is NCMBObject)
						{
							NCMBObject nCMBObject = (NCMBObject)item;
							hashtable.Add(nCMBObject.ObjectId, num);
						}
					}
					IEnumerator enumerator2 = values.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						object current2 = enumerator2.Current;
						if (current2 is NCMBObject)
						{
							NCMBObject nCMBObject2 = (NCMBObject)current2;
							if (hashtable.ContainsKey(nCMBObject2.ObjectId))
							{
								int index = Convert.ToInt32(hashtable[nCMBObject2.ObjectId]);
								arrayList.Insert(index, current2);
							}
							else
							{
								arrayList.Add(current2);
							}
						}
						else if (!arrayList.Contains(current2))
						{
							arrayList.Add(current2);
						}
					}
					NCMBSetOperation operation = new NCMBSetOperation(arrayList);
					_performOperation(key, operation);
				}
				else
				{
					NCMBSetOperation operation2 = new NCMBSetOperation(values);
					_performOperation(key, operation2);
				}
			}
			else
			{
				NCMBAddUniqueOperation operation3 = new NCMBAddUniqueOperation(values);
				_performOperation(key, operation3);
			}
		}

		public void Increment(string key)
		{
			Increment(key, 1L);
		}

		public void Increment(string key, long amount)
		{
			_incrementMerge(key, amount);
		}

		public void Increment(string key, double amount)
		{
			_incrementMerge(key, amount);
		}

		private void _incrementMerge(string key, object amount)
		{
			if (_objectId == null || _objectId.Equals(""))
			{
				object value;
				NCMBSetOperation operation;
				if (estimatedData.TryGetValue(key, out value))
				{
					if (value is string || value == null)
					{
						throw new NCMBException(new InvalidOperationException("Old value is not an number."));
					}
					operation = new NCMBSetOperation(_addNumbers(value, amount));
				}
				else
				{
					operation = new NCMBSetOperation(amount);
				}
				_performOperation(key, operation);
			}
			else
			{
				NCMBIncrementOperation operation2 = new NCMBIncrementOperation(amount);
				_performOperation(key, operation2);
			}
		}

		internal static object _addNumbers(object first, object second)
		{
			try
			{
				object obj = null;
				if (second is long)
				{
					if (first is double)
					{
						first = Math.Truncate(Convert.ToDouble(first));
					}
					return Convert.ToInt64(first) + (long)second;
				}
				return Convert.ToDouble(first) + (double)second;
			}
			catch
			{
				throw new NCMBException(new InvalidOperationException("Value is invalid."));
			}
		}

		internal virtual string _getBaseUrl()
		{
			return NCMBSettings.DomainURL + "/" + NCMBSettings.APIVersion + "/classes/" + ClassName;
		}

		public virtual void DeleteAsync(NCMBCallback callback)
		{
			string url = _getBaseUrl() + "/" + _objectId;
			ConnectType method = ConnectType.DELETE;
			new NCMBConnection(url, method, null, NCMBUser._getCurrentSessionToken()).Connect(delegate(int statusCode, string responseData, NCMBException error)
			{
				try
				{
					if (error != null)
					{
						_handleDeleteResult(false);
					}
					else
					{
						_handleDeleteResult(true);
					}
				}
				catch (Exception error2)
				{
					error = new NCMBException(error2);
				}
				_afterDelete(error);
				if (callback != null)
				{
					callback(error);
				}
			});
		}

		public virtual void DeleteAsync()
		{
			DeleteAsync(null);
		}

		public virtual void SaveAsync(NCMBCallback callback)
		{
			Save(callback);
		}

		public virtual void SaveAsync()
		{
			SaveAsync(null);
		}

		internal void Save()
		{
			Save(null);
		}

		internal void Save(NCMBCallback callback)
		{
			if (!IsDirty)
			{
				if (callback != null)
				{
					callback(null);
				}
				return;
			}
			List<NCMBObject> list = new List<NCMBObject>();
			_findUnsavedChildren(estimatedData, list);
			if (list.Count > 0)
			{
				foreach (NCMBObject item in list)
				{
					try
					{
						item.Save();
					}
					catch (NCMBException error2)
					{
						if (callback != null)
						{
							callback(error2);
						}
						return;
					}
				}
			}
			_beforeSave();
			string text = _getBaseUrl();
			ConnectType method;
			if (_objectId != null)
			{
				text = text + "/" + _objectId;
				method = ConnectType.PUT;
			}
			else
			{
				method = ConnectType.POST;
			}
			IDictionary<string, INCMBFieldOperation> currentOperations = null;
			currentOperations = StartSave();
			string content = _toJSONObjectForSaving(currentOperations);
			new NCMBConnection(text, method, content, NCMBUser._getCurrentSessionToken()).Connect(delegate(int statusCode, string responseData, NCMBException error)
			{
				try
				{
					if (error != null)
					{
						_handleSaveResult(false, null, currentOperations);
					}
					else
					{
						Dictionary<string, object> responseDic = Json.Deserialize(responseData) as Dictionary<string, object>;
						_handleSaveResult(true, responseDic, currentOperations);
					}
				}
				catch (Exception error3)
				{
					error = new NCMBException(error3);
				}
				_afterSave(statusCode, error);
				if (callback != null)
				{
					callback(error);
				}
			});
		}

		internal virtual void _beforeSave()
		{
		}

		internal virtual void _afterSave(int statusCode, NCMBException error)
		{
		}

		internal virtual void _afterDelete(NCMBException error)
		{
		}

		internal string _toJSONObjectForSaving(IDictionary<string, INCMBFieldOperation> operations)
		{
			string text = "";
			lock (mutex)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				foreach (KeyValuePair<string, INCMBFieldOperation> operation in operations)
				{
					INCMBFieldOperation value = operation.Value;
					if (value is NCMBRelationOperation<NCMBObject>)
					{
						NCMBRelationOperation<NCMBObject> nCMBRelationOperation = (NCMBRelationOperation<NCMBObject>)value;
						if (nCMBRelationOperation._relationsToAdd.Count == 0 && nCMBRelationOperation._relationsToRemove.Count == 0)
						{
							continue;
						}
					}
					else if (value is NCMBRelationOperation<NCMBUser>)
					{
						NCMBRelationOperation<NCMBUser> nCMBRelationOperation2 = (NCMBRelationOperation<NCMBUser>)value;
						if (nCMBRelationOperation2._relationsToAdd.Count == 0 && nCMBRelationOperation2._relationsToRemove.Count == 0)
						{
							continue;
						}
					}
					else if (value is NCMBRelationOperation<NCMBRole>)
					{
						NCMBRelationOperation<NCMBRole> nCMBRelationOperation3 = (NCMBRelationOperation<NCMBRole>)value;
						if (nCMBRelationOperation3._relationsToAdd.Count == 0 && nCMBRelationOperation3._relationsToRemove.Count == 0)
						{
							continue;
						}
					}
					dictionary[operation.Key] = NCMBUtility._maybeEncodeJSONObject(value, true);
				}
				return Json.Serialize(dictionary);
			}
		}

		public virtual void FetchAsync(NCMBCallback callback)
		{
			if (_objectId == null || _objectId == "")
			{
				throw new NCMBException("Object ID must be set to be fetched.");
			}
			string url = _getBaseUrl() + "/" + _objectId;
			ConnectType method = ConnectType.GET;
			new NCMBConnection(url, method, null, NCMBUser._getCurrentSessionToken()).Connect(delegate(int statusCode, string responseData, NCMBException error)
			{
				try
				{
					if (error != null)
					{
						_handleFetchResult(false, null);
					}
					else
					{
						Dictionary<string, object> responseDic = Json.Deserialize(responseData) as Dictionary<string, object>;
						_handleFetchResult(true, responseDic);
					}
				}
				catch (Exception error2)
				{
					throw new NCMBException(error2);
				}
				if (callback != null)
				{
					callback(error);
				}
			});
		}

		public virtual void FetchAsync()
		{
			FetchAsync(null);
		}

		public bool ContainsKey(string key)
		{
			lock (mutex)
			{
				return estimatedData.ContainsKey(key);
			}
		}

		public static NCMBObject CreateWithoutData(string className, string objectId)
		{
			NCMBObject nCMBObject = null;
			try
			{
				if (className == "user")
				{
					return new NCMBUser
					{
						ObjectId = objectId
					};
				}
				return new NCMBObject(className)
				{
					ObjectId = objectId
				};
			}
			catch (Exception error)
			{
				throw new NCMBException(error);
			}
		}

		internal virtual void _mergeFromServer(Dictionary<string, object> responseDic, bool completeData)
		{
			lock (mutex)
			{
				IsDirty = false;
				object value;
				if (responseDic.TryGetValue("objectId", out value))
				{
					_objectId = (string)value;
					responseDic.Remove("objectId");
				}
				if (responseDic.TryGetValue("createDate", out value))
				{
					_setCreateDate((string)value);
					responseDic.Remove("createDate");
				}
				if (responseDic.TryGetValue("updateDate", out value))
				{
					_setUpdateDate((string)value);
					responseDic.Remove("updateDate");
				}
				if (!_updateDate.HasValue && _createDate.HasValue)
				{
					_updateDate = _createDate;
				}
				foreach (KeyValuePair<string, object> item in responseDic)
				{
					dataAvailability[item.Key] = true;
					object value2 = item.Value;
					object obj = NCMBUtility.decodeJSONObject(value2);
					if (obj != null)
					{
						serverData[item.Key] = obj;
					}
					else
					{
						serverData[item.Key] = value2;
					}
				}
				if (responseDic.TryGetValue("acl", out value))
				{
					NCMBACL value3 = NCMBACL._createACLFromJSONObject((Dictionary<string, object>)value);
					serverData["acl"] = value3;
					responseDic.Remove("acl");
				}
				_rebuildEstimatedData();
			}
		}

		internal void _handleSaveResult(bool success, Dictionary<string, object> responseDic, IDictionary<string, INCMBFieldOperation> operationBeforeSave)
		{
			lock (mutex)
			{
				if (success)
				{
					_applyOperations(operationBeforeSave, serverData);
					operationSetQueue.Remove(operationBeforeSave);
					_mergeFromServer(responseDic, success);
					_rebuildEstimatedData();
					_updateLatestEstimatedData();
					return;
				}
				LinkedListNode<IDictionary<string, INCMBFieldOperation>> linkedListNode = operationSetQueue.Find(operationBeforeSave);
				IDictionary<string, INCMBFieldOperation> value = linkedListNode.Next.Value;
				operationSetQueue.Remove(linkedListNode);
				foreach (KeyValuePair<string, INCMBFieldOperation> item in operationBeforeSave)
				{
					INCMBFieldOperation value2 = item.Value;
					INCMBFieldOperation value3 = null;
					value.TryGetValue(item.Key, out value3);
					value3 = ((value3 == null) ? value2 : value3.MergeWithPrevious(value2));
					value[item.Key] = value3;
				}
			}
		}

		internal void _handleFetchResult(bool success, Dictionary<string, object> responseDic)
		{
			lock (mutex)
			{
				if (success)
				{
					_mergeFromServer(responseDic, success);
					_rebuildEstimatedData();
				}
			}
		}

		private void _handleDeleteResult(bool success)
		{
			lock (mutex)
			{
				if (success)
				{
					estimatedData["objectId"] = null;
					operationSetQueue.Clear();
					operationSetQueue.AddLast(new Dictionary<string, INCMBFieldOperation>());
					serverData.Clear();
					estimatedData.Clear();
					_dirty = true;
				}
			}
		}

		private void _setCreateDate(string resultDate)
		{
			string format = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";
			_createDate = DateTime.ParseExact(resultDate, format, null);
		}

		private void _setUpdateDate(string resultDate)
		{
			string format = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";
			_updateDate = DateTime.ParseExact(resultDate, format, null);
		}

		internal void _saveToVariable()
		{
			lock (mutex)
			{
				try
				{
					NCMBSettings.CurrentUser = _toJsonDataForDataFile();
				}
				catch (Exception error)
				{
					throw new NCMBException(error);
				}
			}
		}

		internal static NCMBObject _getFromVariable()
		{
			try
			{
				string currentUser = NCMBSettings.CurrentUser;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (currentUser != null && currentUser != "")
				{
					dictionary = Json.Deserialize(currentUser) as Dictionary<string, object>;
					NCMBObject nCMBObject = CreateWithoutData((string)dictionary["className"], null);
					nCMBObject._mergeFromServer(dictionary, true);
					return nCMBObject;
				}
			}
			catch (Exception error)
			{
				throw new NCMBException(error);
			}
			return null;
		}

		internal void _saveToDisk(string fileName)
		{
			string path = NCMBSettings.filePath + "/" + fileName;
			lock (mutex)
			{
				try
				{
					string value = _toJsonDataForDataFile();
					using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8))
					{
						streamWriter.Write(value);
						streamWriter.Close();
					}
				}
				catch (Exception error)
				{
					throw new NCMBException(error);
				}
			}
		}

		internal static NCMBObject _getFromDisk(string fileName)
		{
			try
			{
				string text = _getDiskData(NCMBSettings.filePath + "/" + fileName);
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (text != null && text != "")
				{
					dictionary = Json.Deserialize(text) as Dictionary<string, object>;
					NCMBObject nCMBObject = CreateWithoutData((string)dictionary["className"], null);
					nCMBObject._mergeFromServer(dictionary, true);
					return nCMBObject;
				}
			}
			catch (Exception error)
			{
				throw new NCMBException(error);
			}
			return null;
		}

		internal static string _getDiskData(string fileName)
		{
			try
			{
				string text = "";
				if (File.Exists(fileName))
				{
					StreamReader streamReader = new StreamReader(fileName, Encoding.UTF8);
					using (streamReader)
					{
						string text2;
						do
						{
							text2 = streamReader.ReadLine();
							if (text2 != null)
							{
								text += text2;
							}
						}
						while (text2 != null);
						streamReader.Close();
					}
				}
				return text;
			}
			catch (Exception error)
			{
				throw new NCMBException(error);
			}
		}

		internal string _toJsonDataForDataFile()
		{
			string text = "";
			lock (mutex)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				foreach (KeyValuePair<string, object> serverDatum in serverData)
				{
					object value = serverDatum.Value;
					dictionary[serverDatum.Key] = NCMBUtility._maybeEncodeJSONObject(value, true);
				}
				if (_createDate.HasValue)
				{
					dictionary["createDate"] = NCMBUtility.encodeDate(_createDate.Value);
				}
				if (_updateDate.HasValue)
				{
					dictionary["updateDate"] = NCMBUtility.encodeDate(_updateDate.Value);
				}
				if (_objectId != null)
				{
					dictionary["objectId"] = _objectId;
				}
				dictionary["className"] = _className;
				return Json.Serialize(dictionary);
			}
		}

		private void _setDefaultValues()
		{
			if (NCMBACL._getDefaultACL() != null)
			{
				ACL = NCMBACL._getDefaultACL();
			}
		}
	}
}
