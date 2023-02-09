using System;
using System.Collections;
using System.Collections.Generic;

namespace NCMB.Internal
{
	internal class NCMBRelationOperation<T> : INCMBFieldOperation where T : NCMBObject
	{
		private string _targetClass;

		internal HashSet<string> _relationsToAdd;

		internal HashSet<string> _relationsToRemove;

		internal string TargetClass
		{
			get
			{
				return _targetClass;
			}
		}

		internal NCMBRelationOperation(HashSet<T> newRelationsToAdd, HashSet<T> newRelationsToRemove)
		{
			_targetClass = null;
			_relationsToAdd = new HashSet<string>();
			_relationsToRemove = new HashSet<string>();
			if (newRelationsToAdd != null)
			{
				foreach (T item in newRelationsToAdd)
				{
					if (item.ObjectId == null)
					{
						throw new NCMBException(new ArgumentException("All objects in a relation must have object ids."));
					}
					_relationsToAdd.Add(item.ObjectId);
					if (_targetClass == null)
					{
						_targetClass = item.ClassName;
					}
					else if (!_targetClass.Equals(item.ClassName))
					{
						throw new NCMBException(new ArgumentException("All objects in a relation must be of the same class."));
					}
				}
			}
			if (newRelationsToRemove != null)
			{
				foreach (T item2 in newRelationsToRemove)
				{
					if (item2.ObjectId == null)
					{
						throw new NCMBException(new ArgumentException("All objects in a relation must have object ids."));
					}
					_relationsToRemove.Add(item2.ObjectId);
					if (_targetClass == null)
					{
						_targetClass = item2.ClassName;
					}
					else if (!_targetClass.Equals(item2.ClassName))
					{
						throw new NCMBException(new ArgumentException("All objects in a relation must be of the same class."));
					}
				}
			}
			if (_targetClass == null)
			{
				throw new NCMBException(new ArgumentException("Cannot create a NCMBRelationOperation with no objects."));
			}
		}

		private NCMBRelationOperation(string newTargetClass, HashSet<string> newRelationsToAdd, HashSet<string> newRelationsToRemove)
		{
			_targetClass = newTargetClass;
			_relationsToAdd = new HashSet<string>(newRelationsToAdd);
			_relationsToRemove = new HashSet<string>(newRelationsToRemove);
		}

		public object Encode()
		{
			Dictionary<string, object> dictionary = null;
			Dictionary<string, object> dictionary2 = null;
			if (_relationsToAdd.Count > 0)
			{
				dictionary = new Dictionary<string, object>();
				dictionary.Add("__op", "AddRelation");
				dictionary.Add("objects", _convertSetToArray(_relationsToAdd));
			}
			if (_relationsToRemove.Count > 0)
			{
				dictionary2 = new Dictionary<string, object>();
				dictionary2.Add("__op", "RemoveRelation");
				dictionary2.Add("objects", _convertSetToArray(_relationsToRemove));
			}
			if (dictionary != null)
			{
				return dictionary;
			}
			if (dictionary2 != null)
			{
				return dictionary2;
			}
			return null;
		}

		private ArrayList _convertSetToArray(HashSet<string> set)
		{
			ArrayList arrayList = new ArrayList();
			foreach (string item in set)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("__type", "Pointer");
				dictionary.Add("className", _targetClass);
				dictionary.Add("objectId", item);
				arrayList.Add(dictionary);
			}
			return arrayList;
		}

		public INCMBFieldOperation MergeWithPrevious(INCMBFieldOperation previous)
		{
			if (previous == null)
			{
				return this;
			}
			if (previous is NCMBDeleteOperation)
			{
				throw new NCMBException(new ArgumentException("You can't modify a relation after deleting it."));
			}
			if (previous is NCMBRelationOperation<T>)
			{
				NCMBRelationOperation<T> nCMBRelationOperation = (NCMBRelationOperation<T>)previous;
				if (nCMBRelationOperation._targetClass != null && !nCMBRelationOperation._targetClass.Equals(_targetClass))
				{
					throw new NCMBException(new ArgumentException("Related object object must be of class " + nCMBRelationOperation._targetClass + ", but " + _targetClass + " was passed in."));
				}
				HashSet<string> hashSet = new HashSet<string>(nCMBRelationOperation._relationsToAdd);
				HashSet<string> hashSet2 = new HashSet<string>(nCMBRelationOperation._relationsToRemove);
				if (_relationsToAdd.Count > 0)
				{
					if (hashSet2.Count == 0)
					{
						foreach (string item in _relationsToAdd)
						{
							hashSet.Add(item);
						}
					}
					else
					{
						foreach (string item2 in _relationsToAdd)
						{
							hashSet2.Remove(item2);
						}
					}
				}
				if (_relationsToRemove.Count > 0)
				{
					if (hashSet.Count == 0)
					{
						foreach (string item3 in _relationsToRemove)
						{
							hashSet2.Add(item3);
						}
					}
					else
					{
						foreach (string item4 in _relationsToRemove)
						{
							hashSet.Remove(item4);
						}
					}
				}
				return new NCMBRelationOperation<T>(_targetClass, hashSet, hashSet2);
			}
			throw new NCMBException(new ArgumentException("Operation is invalid after previous operation."));
		}

		public object Apply(object oldValue, NCMBObject obj, string key)
		{
			if (oldValue == null || (oldValue is IList && ((IList)oldValue).Count == 0))
			{
				return new NCMBRelation<T>(obj, key)
				{
					TargetClass = _targetClass
				};
			}
			if (oldValue is NCMBRelation<T>)
			{
				NCMBRelation<T> nCMBRelation = (NCMBRelation<T>)oldValue;
				if (_targetClass != null && nCMBRelation.TargetClass != null)
				{
					if (!nCMBRelation.TargetClass.Equals(_targetClass))
					{
						throw new ArgumentException("Related object object must be of class " + nCMBRelation.TargetClass + ", but " + _targetClass + " was passed in.");
					}
					nCMBRelation.TargetClass = _targetClass;
				}
				return nCMBRelation;
			}
			throw new NCMBException(new ArgumentException("Operation is invalid after previous operation."));
		}
	}
}
